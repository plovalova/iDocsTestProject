// Create DB and collection
db = new Mongo().getDB("DocumentsDb");
db.createCollection("docs");