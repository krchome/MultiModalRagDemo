
using MultiModalRagDemo.Models;
using MultiModalRagDemo.Services;
//using MultiModalRagDemo.Services;



namespace MultiModalRagDemo.Services
    {
        public class TextChunkingService : ITextChunkingService
        {
            public List<TextChunk> ChunkText(
                string text,
                int chunkSize = 800,
                int overlapSize = 100)
            {
                var chunks = new List<TextChunk>();

                if (string.IsNullOrWhiteSpace(text))
                {
                    return chunks;
                }

                if (chunkSize <= 0)
                {
                    throw new ArgumentException("Chunk size must be greater than zero.", nameof(chunkSize));
                }

                if (overlapSize < 0)
                {
                    throw new ArgumentException("Overlap size cannot be negative.", nameof(overlapSize));
                }

                if (overlapSize >= chunkSize)
                {
                    throw new ArgumentException("Overlap size must be smaller than chunk size.", nameof(overlapSize));
                }

                var normalizedText = NormalizeWhitespace(text);

                var startIndex = 0;
                var chunkIndex = 1;

                while (startIndex < normalizedText.Length)
                {
                    var remainingLength = normalizedText.Length - startIndex;
                    var currentChunkSize = Math.Min(chunkSize, remainingLength);

                    var chunkContent = normalizedText
                        .Substring(startIndex, currentChunkSize)
                        .Trim();

                    if (!string.IsNullOrWhiteSpace(chunkContent))
                    {
                        chunks.Add(new TextChunk
                        {
                            ChunkIndex = chunkIndex,
                            Content = chunkContent
                        });

                        chunkIndex++;
                    }

                    if (startIndex + currentChunkSize >= normalizedText.Length)
                    {
                        break;
                    }

                    startIndex += chunkSize - overlapSize;
                }

                return chunks;
            }

            private static string NormalizeWhitespace(string text)
            {
                return string.Join(
                    " ",
                    text.Split(
                        new[] { ' ', '\r', '\n', '\t' },
                        StringSplitOptions.RemoveEmptyEntries));
            }
        }
    }
