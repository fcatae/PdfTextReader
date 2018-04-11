O processamento é separado em 3 etapas:
- Blockset
- Segmentation
- Semantics

Durante essas etapas, são realizados os seguintes processos:
- Extração de blocos dos PDF
- Transformação de blocos em BlockSets
- Transformação de blocos em TextLines
- Criação de segmentos sequenciais
- Geração de artigos

# Arquivos

- Page:
    - PDF ou Blocks
    - Blockset.json

- Documento
    - TextLines

- Artigos
    - Sequence
    - Articles

------------------------------------------

# Stage 1

Transform PDF into JSON Blockset

# Stage 2: extra content

Transform PNG + Json into PNG

# Stage 3: create article

Use JSON to generate XML
