# 📝 Update

## Overview

MongoDB provides a set of update operators to help you update values on your collections' documents. This section contains several samples showing how to use these operators to update:

* top level fields
* embedded fields
* array fields

{% hint style="info" %}
Complex update operations such as updating embedded fields, adding or removing items in embedded array fields might be straight forward when building the queries in the Shell but not so much when using the MongoDB C\# driver
{% endhint %}

The section contains samples for the following operators:

| Operator | Description |
| :--- | :--- |
| **Set** | Sets the value of a field in a document |
| **Inc** | Increments a field's value |
| **Min** | Updates a field's value only if the specified value is **less** than the existing |
| **Max** | Updates a field's value only if the specified value is **greater** than the existing |
| **Mul** | Multiplies a field's value by a specified amount |
| **Unset** | Removes the specified field from the document |
| **Rename** | Renames a field |

Other than the update operations there are also samples showing how to **replace** documents and **array** fields.

![](../.gitbook/assets/update.png)

