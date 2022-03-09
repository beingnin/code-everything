const admin = require('firebase-admin');
const firebase = require('firebase/app');
const { signInWithEmailAndPassword, getAuth } = require('firebase/auth')
const serviceAccount = require('../firebase-admin.json')

const firebaseConfig = {
    apiKey: "AIzaSyAip63iwTQXC1v2koJ6rwAs8gFkq8_iy_A",
    authDomain: "spsa-firebase.firebaseapp.com",
    projectId: "spsa-firebase",
    storageBucket: "spsa-firebase.appspot.com",
    messagingSenderId: "938074650119",
    appId: "1:938074650119:web:872e4573a1071845449ca2",
    measurementId: "G-M2XZCVMQSR"
  };
firebase.initializeApp(firebaseConfig);

console.log('firebase client app initialized')

admin.initializeApp(
    {
        credential: admin.credential.cert(serviceAccount)
    });
console.log('firebase admin app initialized');


const db = admin.firestore();

const authApp = admin.auth();
db.settings = ({ timestampsInSnapshots: true });
// const app = initializeApp(firebaseConfig);

module.exports = { db, authApp, signInWithEmailAndPassword, getAuth }