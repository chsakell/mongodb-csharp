# Array operators

## Overview

There are many times when you want to match documents based on array field values. Luckily, MongoDB and the C\# driver provide the following 3 array query operators that help you build array based queries.

* **Size** _\(or Count\)_ operator
* **ElemMatch** operator
* **All** operator

{% hint style="info" %}
These operators may seem simple at first 😇 , but when combined with other MongoDB features such as **projection** or **unwind**, you will see that you can build quite complex queries! 💪 
{% endhint %}

![Array operators](../.gitbook/assets/arrays.png)

