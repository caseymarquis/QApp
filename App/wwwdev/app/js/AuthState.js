import Vue from "vue";
import Md5 from "./Md5.js";
import axios from 'axios';
import Env from './Environment.js';

const tokenStorageName = "AuthState.token";

const AuthState = {
    bus: new Vue(),
    token: null,
    //Many of our users don't use SSL, so we avoid clear
    //text with an irreversible hash here. It's better than
    //nothing, but vulnerable to weak password attacks.
    //The goal of all of this is to prevent attackers from figuring out what the original password was.
    hashPassword(username, password) {
        let promise = new Promise((resolve, reject) => {
            axios.post(`Auth/Salt?username=${username}`).then(function (response) {
                //Per install and user unique salt so you can't crack passwords for every install or user at once.
                if (response.data === null) {
                    reject(new Error("User does not exist"));
                }
                else {
                    let salt = response.data.toLowerCase().trim();
                    password = Md5(password + salt);
                    resolve(password);
                }
            }).catch(function (ex) {
                reject(ex);
            });
        });
        return promise;
    },
    updateAuthToken(username, password, $store) {
        $store.commit("clearLoginMessage");
        let hashFunction = AuthState.hashPassword;
        hashFunction(username, password).then((password) => {
            let appendUrl = '';
            axios.post(`Auth/LoginUser?username=${username}&password=${password}`).then(function (response) {
                let user = response.data;
                if (user !== null && user.token !== undefined && user.token !== null) {
                    AuthState.token = user.token;
                    Env.setApiRoot(user.token);
                    localStorage.setItem(tokenStorageName, user.token);
                    $store.commit('login', user);
                }
                else {
                    $store.commit('loginFailed', "Wrong password.");
                }
            }).catch(function (ex) {
                $store.commit('loginFailed', ex.message);
            });
        }).catch(function (ex) {
            $store.commit('loginFailed', ex.message);
        });
    },
    tryToLogInWithOldAuthToken($store) {
        let token = localStorage.getItem(tokenStorageName, token);
        if (token !== undefined && token !== null) {
            AuthState.token = token;
            Env.setApiRoot(token);
            axios.post(`Auth/GetUserFromToken`).then(function (response) {
                let user = response.data;
                if (user !== null) {
                    $store.commit('login', user);
                }
                else {
                    $store.commit('loginFailed', "Existing session expired.");
                }
            }).catch(function (ex) {
                $store.commit('loginFailed', ex.message);
            });
        }
    },
    logOut($store) {
        localStorage.removeItem(tokenStorageName);
        $store.commit('logout');
    }
}


export default AuthState;
