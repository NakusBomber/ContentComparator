# Lab Work №1: Web Page Comparison Using Cosine Similarity

### Course: Internet Technologies

### Topic: Comparing Web Pages Based on Cosine Similarity

## Objective
The goal of this lab work is to develop a software tool for determining the similarity of web pages based on their content using cosine similarity, implemented in C#.

## Task Breakdown
1. **Find Websites with Similar Topics**  
   Select at least 5 websites with similar themes to compare.

2. **Develop a Program to Calculate TF, IDF, and TF-IDF**  
   Implement a program that computes the term frequency (TF), inverse document frequency (IDF), and term frequency-inverse document frequency (TF-IDF) values for the content of the websites.

3. **Compare the Similarity of the Websites**  
   Using cosine similarity, compare the thematic similarity between the websites.

## Theoretical Background

### TF-IDF
TF-IDF (Term Frequency-Inverse Document Frequency) is a statistical measure used to evaluate the importance of a word in a document relative to a collection (corpus) of documents. The importance of a word increases with the number of times it appears in a document but decreases with its frequency in the entire corpus.

- **TF (Term Frequency)**  
  TF measures the frequency of a term in a document:
  
  <center><img src="https://latex.codecogs.com/svg.image?TF(t_i)=\frac{n_i}{\sum_{j}n_j}"/></center>
  
  where n_i is the number of occurrences of term t_i in the document, and sum is the total number of words in the document.

- **IDF (Inverse Document Frequency)**  
  IDF measures how common or rare a term is across all documents:
  
  <center><img src="https://latex.codecogs.com/svg.image?IDF(t_i)=\log\left(\frac{|D|}{|d_i|}\right)"/></center>
  
  where |D| is the total number of documents in the corpus, and |d_i| is the number of documents that contain the term t_i. The logarithm base can vary, but it only affects the scale, not the relative importance of terms.

- **TF-IDF**  
  The TF-IDF score is the product of the TF and IDF values:
  
  <center><img src="https://latex.codecogs.com/svg.image?TF\text{-}IDF(t_i)=TF(t_i)\times&space;IDF(t_i)"/></center>
  
  Terms with a high TF in a document but low occurrence across the corpus have the highest TF-IDF score.

### Cosine Similarity
Cosine similarity is a measure of similarity between two non-zero vectors in a pre-Hilbert space, calculated as the cosine of the angle between them. It evaluates the orientation rather than the magnitude of the vectors.

For two vectors (A) and (B), cosine similarity is given by:

<center><img src="https://latex.codecogs.com/svg.image?\text{cosine&space;similarity}=\cos(\theta)=\frac{A\cdot&space;B}{\|A\|\cdot\|B\|}"/></center>

- If the cosine similarity is 1, the vectors are identical in direction.
- A cosine similarity of 0 indicates orthogonality (no similarity).
- The cosine similarity ranges between [0, 1] in positive spaces, which makes it a useful metric for text analysis.

## Steps
1. **Select Websites:** Choose websites with a common theme to analyze.
2. **Text Preprocessing:** Extract and clean the content from the selected websites.
3. **TF-IDF Calculation:** Implement the algorithm in C# to calculate TF, IDF, and TF-IDF for the websites' content.
4. **Cosine Similarity Computation:** Use cosine similarity to compare the TF-IDF vectors of the website content.
5. **PDF Report Generation:** Generate a report of the results using the MigraDoc library in C#.

## Tools and Libraries
- **Programming Language:** C#
- **Libraries:** `MigraDoc` for generating PDF reports.
