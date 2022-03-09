const authenticationRepo = require('../repositories.firebase/authentication')
const express = require("express");
const routes = express.Router();

routes.route('/authentication/gettoken').post(async (req, res) => {

    try {
        res.json(await authenticationRepo.getToken(req.body.email,req.body.password));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }
})

module.exports=routes;
