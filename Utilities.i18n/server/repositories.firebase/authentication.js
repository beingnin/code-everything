const { authApp, signInWithEmailAndPassword, getAuth } = require('./firebaseAppBase');


const getToken = async (email, password) => {

    //  let user = await authApp.getUserByEmail(email);

    // let token =  await authApp.createCustomToken(user.uid, { email: email, role: 'developer' });

    let auth = getAuth();


    let role = '';

    switch (email.split('@')[1]) {

        case 'pitsolutions.com':
            role = 'developer';
            break;

        case 'shjpolice.gov.ae':
            role = 'translator';
            break;

        default:
            break;

    }
    let user = await signInWithEmailAndPassword(auth, email, password);
    await authApp.setCustomUserClaims(user.user.uid, { role: role });

    return { token: user._tokenResponse.idToken, email: email, role: role };

}
const authenticate = async (token) => {

    try {
        let claims = await authApp.verifyIdToken(token);
        return claims;
    }
    catch (error) {
        return null;
    }

}
module.exports = { getToken, authenticate }