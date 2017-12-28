Architecture
=============

1. PDF Parser
2. Semantic Tagging
3. Linker


Current Status
===============

Work in progress:
1. Rewriting the parse module
2. Brainstorming the semantic tagging
3. Working on the Linker


# 1. Rewriting the parse module #

`UserWriter` contains major codebase separated into 4 different categories.

### MainLogic ###

Process
- ActiveTables
- ProcessBlockExtra
- ProcessBlock
- FinalProcess

### Blocklist ###
  
Structure
- TryMergeBlockSets
- RemoveList

Region
- FindHeader
- FindFooter

Visual
- DrawRectangle (depends on PDFCanvas)
- PrintText

Complex code
- BreakBlockSets
- CalculateCenterBreak

### BlockSet ###

Calculation
- GetBlockWithLargerWidth
- GetBlockWithSmallerWidth
- HasAreaOverlap
- HasOverlap(segA, segB)
- HasOverlap(area,x,y)

Table
- IsInTable

### StructureItem ###

- ProcessStructure
- HighlightStructureItems

