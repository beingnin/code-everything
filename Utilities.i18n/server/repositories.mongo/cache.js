let json = {

}

const set = (key, data) => json[key] = JSON.stringify(data);

const invalidate = (keys) => {
    keys = new Set(keys)
    if (keys && keys.size > 0) {
        keys.forEach(k => { json[k] = null; console.log(`cache cleared for key ${k}`); });
    }
}

const get = (key) => json[key];

module.exports = { set, get, invalidate };