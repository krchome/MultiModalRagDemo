import os
from typing import List, Optional
 
from fastapi import FastAPI, HTTPException
from pydantic import BaseModel
 
try:
	from sentence_transformers import SentenceTransformer
except Exception as e:
	raise RuntimeError(
		"sentence-transformers is required. Install with: pip install -r requirements.txt" ) from e
 
app = FastAPI(title="Embedding Service", version="0.1")
 
MODEL_NAME = os.getenv("MODEL_NAME", "all-MiniLM-L6-v2")
 
# Load model at startup
try:
	model = SentenceTransformer(MODEL_NAME)
except Exception as ex:
	raise RuntimeError(f"Failed to load model '{MODEL_NAME}': {ex}") from ex
 
 
class EmbedRequest(BaseModel):
	# Accept either a single text or a list of texts
	texts: Optional[List[str]] = None
	text: Optional[str] = None
 
 
class EmbedResponse(BaseModel):
	embeddings: List[List[float]]
 
 
@app.get("/health")
async def health():
	return {"status": "ok", "model": MODEL_NAME}
 
 
@app.post("/embed", response_model=EmbedResponse)
async def embed(req: EmbedRequest):
	# Normalize input to a list of strings
	if req.text is None and req.texts is None:
		raise HTTPException(status_code=400, detail="Provide 'text' or 'texts' in request body")
 
	if req.texts is None:
		texts = [req.text]
	else:
		texts = req.texts
 
	# Basic validation
	if not isinstance(texts, list) or not all(isinstance(t, str) for t in texts):
		raise HTTPException(status_code=400, detail="'texts' must be a list of strings")
 
	# Compute embeddings
	try:
		embeddings = model.encode(texts, show_progress_bar=False, convert_to_numpy=True)
	except Exception as e:
		raise HTTPException(status_code=500, detail=f"Embedding failure: {e}")
 
	# Convert numpy arrays to plain Python lists for JSON serialization
	embeddings_list = [emb.tolist() for emb in embeddings]
	return {"embeddings": embeddings_list}

