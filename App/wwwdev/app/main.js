import 'babel-polyfill';

import 'jquery';
import Bootstrap from 'bootstrap';
import './scss/bootswatch.scss';

import Vue from 'vue';
import VueRouter from 'vue-router';

//Import on main page load:
import MainRouter from './site/main-router.vue';

//Import when navigating to page:
const MainLayout = () => import('./site/main-layout.vue');
const PageHome = () => import('./page-home/page-home.vue');
const PageUsers = () => import('./page-users/page-users.vue');
const PageUser = () => import('./page-user/page-user.vue');

import Env from './js/Environment';

import store from './js/store.js';

Vue.use(VueRouter);

Env.setApiRoot(null);

const router = new VueRouter({
    routes: [
        { path: "/", component : MainLayout,
            children : [
                { path: "", component : PageHome,
                },
                { path: "users", component : PageUsers,
                },
                { path: "user/:id", component : PageUser,
                },
            ]
        },
    ]
});

var vm = new Vue({
    el: '#router-mount',
    render: h => h(MainRouter),
    router,
    store
});