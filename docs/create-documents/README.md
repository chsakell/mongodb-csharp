# ➕ Create

## 🆕 New documents

Inserting new documents is quite easy as explained in the Quick Start [Insert Documents](../getting-started/quick-start/insert-documents.md) section. You can use the `InsertOne` and `InsertMany` methods to add one or multiple documents respectively. Other than the basics, it's certain you will face some more advanced scenarios, such as inserting nested documents or pushing items to nested array fields. You might think this is an insert operation but for MongoDB are considered update operations and they will be described on the related sections.

## 🛠 Configuration

Each top level document in MongoDB contains an `_id` field which is either passed as an argument during insertion either automatically created. This field can be generated and deserialized back to a C\# model class based on a related configuration which the [Id Membe](id-member.md)r page covers thoroughly. 

## Ordered Insert

Another concept covered in this section is the [Ordered Insert](ordered-insert.md), which explains how MongoDB behaves when facing unique ids conflicts during multiple documents insertions.



