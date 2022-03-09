const express = require('express');
const authentication = require('./repositories.firebase/authentication')
const path = require('path');
const app = express();
const cors = require("cors");
require("dotenv").config({ path: "./config.env" });
const port = process.env.PORT || 5000;

// app.use((req, res, next)=>{

//   var oldWrite = res.write,
//       oldEnd = res.end;

//   var chunks = [];

//   res.write = function (chunk) {
//     chunks.push(chunk);

//     return oldWrite.apply(res, arguments);
//   };

//   res.end = function (chunk) {
//     if (chunk)
//       chunks.push(chunk);

//     var body = Buffer.concat(chunks).toString('utf8');
//     console.log(req.path, body);

//     oldEnd.apply(res, arguments);
//   };

//   next();
// })
app.use(cors());
app.use(express.json({ limit: '200mb' }));
app.use(express.static(path.join(__dirname, 'server', '..', 'public'), { index: 'index.html' }));

let whitelisturls = [
  '/authentication/gettoken',
  '/internationalization/search'
]

//authentication middleware
app.use(async (req, res, next) => {
  if (req.method.toUpperCase() === 'GET') {
    next();
  }
  else if (whitelisturls.filter((url) => req.originalUrl.replace(/\/+/g,'/').includes(url)).length > 0) {
    next();
  }
  else {
    if (!req.headers.authorization) {
      res.statusMessage = 'You are not authorized to perform this action';
      res.sendStatus(401);
    }
    let token = req.headers.authorization.split(' ')[1];
    let claims = await authentication.authenticate(token);
    if (claims) {
      req.claims = claims;
      next();
    }
    else {
      res.statusMessage = 'You are not authorized to perform this action';
      res.sendStatus(401);
    }
  }

  });
//routes
app.use(require('./routes/internationalization'));
app.use(require('./routes/authentication'))

app.listen(port, () => {

  console.log(`Server is running on port: ${port}`);
});