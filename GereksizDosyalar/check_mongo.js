const MongoClient = require('mongodb').MongoClient;
const url = 'mongodb://localhost:27017';
const dbName = 'StoreMD';
MongoClient.connect(url, function(err, client) {
  if (err) throw err;
  const db = client.db(dbName);
  db.collection('Brands').find({}).toArray(function(err, result) {
    if (err) throw err;
    console.log(JSON.stringify(result));
    client.close();
  });
});
