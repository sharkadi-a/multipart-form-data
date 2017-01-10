# HTML Multi-part for data parsing and content decoding
This repository was initially created for personal needs, but later developed into own open-source solution. 
MultiPartFormDataParse contains multiple classes for:
* Parsing raw HTML multipart/form-data into array of form items;
* Transforming (decoding) each item into specific CLR data type (basically, string, binary data, image, common xml deserializer) for Content-Type header;
* Content transfer encoding content transformation into bytes or chars. Content-Transfer-Encoding header is used by transfer environments which treat data as a stream of characters, not bytes;
* Factories for Content-Type and Content-Transfer-Encoding content decoders;
* Charset into Encoding resolver (for custom charsets).
Code is not 100% covered with unit tests, but overall shall work pretty descent. More unit tests featured.
