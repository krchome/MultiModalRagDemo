import os
import argparse
import json
from typing import List
 
import requests
 
 
def load_chunks_from_dir(chunks_dir: str) -> List[dict]:
	chunks = []
	for fname in os.listdir(chunks_dir):
		path = os.path.join(chunks_dir, fname)
		if os.path.isfile(path) and path.lower().endswith('.json'):
			with open(path, 'r', encoding='utf-8-sig') as f:
				data = json.load(f)
				# Expect each file to contain a dict with 'text' and 'meta'
				chunks.append({'id': fname, 'text': data.get('text'), 'meta': data.get('meta', {})})
	return chunks
 
 
def embed_chunks(embed_url: str, chunks: List[dict]):
	texts = [c['text'] for c in chunks]
	resp = requests.post(embed_url, json={'texts': texts})
	resp.raise_for_status()
	return resp.json().get('embeddings', [])
 
 
def main():
	parser = argparse.ArgumentParser()
	parser.add_argument('--chunks', required=True, help='Path to directory with chunk JSON files')
	parser.add_argument('--embed-url', default='http://localhost:8000/embed', help='Embedding service URL')
	args = parser.parse_args()
 
	chunks = load_chunks_from_dir(args.chunks)
	print(f'Found {len(chunks)} chunks')
	if not chunks:
		return
 
	embeddings = embed_chunks(args.embed_url, chunks)
	print(f'Received {len(embeddings)} embeddings')
 
	# For demo, write out embeddings next to chunks
	out_dir = os.path.join(args.chunks, 'with_embeddings')
	os.makedirs(out_dir, exist_ok=True)
	for chunk, emb in zip(chunks, embeddings):
		out = {'id': chunk['id'], 'text': chunk['text'], 'meta': chunk['meta'], 'embedding': emb}
		with open(os.path.join(out_dir, chunk['id']), 'w', encoding='utf-8') as f:
			json.dump(out, f)
 
	print('Wrote embeddings to', out_dir)
 
 
if __name__ == '__main__':
	main()

