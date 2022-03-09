const privileges = {
    language:
    {
        add: ['developer'],
        update:
        {
            en: ['developer'],
            ar: '*',
            usage: ['developer']
        },
        delete: ['developer'],
        migrate:['developer']
    }
}

const authorize = (action, role) => {
    let splits = action.split('.');
    action = privileges;
    for(let i of splits){
        action = action[i];
    }
    console.log(action)
    return action === '*' || action.filter(x => x === role).length > 0;
}
module.exports = authorize;