# Overview

## Aggregation framework

Aggregation framework in MongoDB is a powerful feature that allows you to build complex queries that you normally cannot build with the common operators and functions. The framework achieves this by putting your documents into a multi-stage pipeline that transforms the documents in the desired result.

### Pipeline

The pipeline consists of a series of [stages](https://docs.mongodb.com/manual/reference/operator/aggregation-pipeline/#aggregation-pipeline-operator-reference) where each stage receives the output of the previous stage. The section contains samples for the following stages:

| Stage | Description |
| :--- | :--- |
| **Project** | Adds new fields or removes existing ones |
| **Match** | Filters the documents |
| **Group** | Groups documents by an identifier expression providing accumulator results |
| **Unwind** | Deconstructs an array field |
| **Pagination** _\(limit, skip\)_ | Skips the first _n_ documents and limits the total results |
| **Bucket** | Produces a distribution among documents based on a specified expression and specified boundaries |

![Aggregation framework](../.gitbook/assets/aggregation.png)

