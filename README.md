# SearchEngine

This class library and example project includes various components to create a search engine for content with indexing, spell checking, word stemming, relevancy ordering, hit highlighting and pagination.

This solution can be used to index content immediately as it is created or updated, rather than waiting for scheduled tasks.

To use this with a database, you would require a column for the indexed version of the searchable content. A VARCHAR(MAX) would be most appropriate.

When creating or updating content you would re/build this index using the Indexing.Generate Index method.

See Default.aspx in the example project for how this system can be used.

Note that the example project does not use a database. Instead it uses a List and generates and stores the index in memory. In a real application you would store this index in a database with your content.
