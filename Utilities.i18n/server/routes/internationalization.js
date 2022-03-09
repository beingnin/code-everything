const { json } = require("express");
const express = require("express");
const langRepo = require('../repositories.mongo/languageRepo');
const history = require('../repositories.mongo/languageHistoryRepo');

const routes = express.Router();


routes.route('/internationalization/findby/key/:key').get(async (req, res) => {

    try {
        res.json(await langRepo.getByKey(req.params.key));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }

})

routes.route('/internationalization/findby/id/:id').get(async (req, res) => {

    try {
        res.json(await langRepo.getById(req.params.id));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }

})

routes.route('/internationalization/findby/similarity/:text').get(async (req, res) => {

    try {
        res.json(await langRepo.similarity(req.params.text));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }


})

routes.route('/internationalization/suspect').get(async (req, res) => {

    try {
        res.json(await langRepo.suspect());
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }


})

routes.route('/internationalization/search').post(async (req, res) => {

    try {
        res.json(await langRepo.search(req.body, req.query.page, req.query.size));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }

})

routes.route('/internationalization/export/:usage').get(async (req, res) => {

    try {
        let result = await langRepo.json(req.params.usage);

        let buffer = Buffer.from(result, 'utf8');
        res.set('Content-Type', 'application/json; charset=UTF-8');
        res.set('Content-Length', buffer.length);
        res.set('Content-disposition', 'attachment;filename=phrases(' + req.params.usage + ').json');
        res.send(buffer);
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }
})

routes.route('/internationalization/add').post(async (req, res) => {

    try {
        res.json(await langRepo.add(req.body, req.claims.email, req.claims.role));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }

})

routes.route('/internationalization/update').post(async (req, res) => {

    try {
        res.json(await langRepo.update(req.body, req.claims.email, req.claims.role));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }
})

routes.route('/internationalization/migrate/:usage').post(async (req, res) => {

    try {
        res.json(await langRepo.migrate(req.body, req.params.usage, req.claims.email, req.claims.role));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }
})

routes.route('/internationalization/delete/:id').post(async (req, res) => {

    try {
        res.json(await langRepo.deletePhrase(req.params.id, req.claims.role, req.claims.email));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }
})


//history
routes.route('/internationalization/getHistory/:originalId').get(async (req, res) => {
    try {
        res.json(await history.get(req.params.originalId));
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }
})

routes.route('/internationalization/getdeleted').get(async (req, res) => {
    try {
        res.json(await history.getDeleted());
    }
    catch (error) {
        res.statusMessage = error.toString();
        res.sendStatus(500);
    }
})






module.exports = routes;