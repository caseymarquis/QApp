<template>
    <div>
        <slide-menu>
        </slide-menu>
        <div class="navbar navbar-default navbar-fixed-top">
            <div class="container navbar-container the-nav-container">
                <div class="navbar-header">
                    <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                        <span class="sr-only">Toggle navigation</span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                        <span class="icon-bar"></span>
                    </button>
                </div>
                <div class="navbar-collapse collapse">
                    <ul class="nav navbar-nav">
                        <li>
                            <router-link to="/" style="padding-top: 4px; padding-bottom: 0; padding-left:0; padding-right: 0;">
                                <!--img :src="imgLogo" class="center-block visible-lg" style="height: 70px; margin-left: 0;" /-->
                                <!--img :src="imgLogo" class="center-block visible-md visible-sm" style="max-height: 35px; margin-left: 0; margin-top: 15px;" /-->
                            </router-link>
                        </li>
                        <li>
                            <div class="page-title-div">
                                <h2 class="page-title visible-lg" v-text="pageTitle">
                                </h2>
                            </div>
                        </li>
                        <li v-if="loggedIn">
                            <button class="btn main-search" :class="{ 'btn-primary': !machineFilterActive, 'btn-warning': machineFilterActive }" v-on:click="toggleSlide">
                                <span class="glyphicon glyphicon-list"></span>
                            </button>
                        </li>
                        <li v-if="loggedIn && $store.state.search.show">
                            <search-bar v-model="searchText" class="main-search" placeholder="Search..."></search-bar> 
                        </li>
                    </ul>
                    <ul v-if="loggedIn" class="nav navbar-nav navbar-right">
                        <li>
                            <router-link :to="'/user/' + $store.state.user.id" class="right-nav-item">
                                <h2 v-text="userFirstName">
                                </h2>
                            </router-link>
                        </li>
                        <li>
                            <a class="right-nav-item">
                                <h2 v-on:click="logout">
                                    Log Out
                                </h2>
                            </a>
                        </li>
                        <li>
                            <a class="right-nav-item">
                                <router-link to="/" class="rlink-top-menu">
                                    <h2>
                                        Home
                                    </h2>
                                </router-link>
                            </a>
                        </li>
                    </ul>
                </div>
            </div>
        </div>

        <div class="container main-container">
            <div class="col-sm-12 main-column">
                <div>
                    <page-home v-if="!loggedIn"></page-home>
                    <router-view v-else></router-view>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import PageHome from '../page-home/page-home.vue';
import SearchBar from '../shared-components/search-bar.vue';
import SlideMenu from './slide-menu.vue';
//import imgLogo from "../img/logo.png";

import AuthState from '../js/AuthState.js';
import SetupPage from "../js/SetupPage.js";
import router from "../js/router.js";

export default {
    data() {
        return {
            //imgLogo,
            wasMounted: false,
        };
    },
    mounted(){
        this.wasMounted = true;
    },
    created() {
        AuthState.tryToLogInWithOldAuthToken(this.$store);
    },
    computed: {
        loggedIn() {
            return this.$store.state.loggedIn;
        },
        userFirstName() {
            let name = this.$store.state.user.name;
            if (name == undefined || name == null) {
                return "";
            }
            name = name.trim();
            let parts = name.split(" ");
            let ret = "";
            if (parts.length === 1) {
                return parts[0];
            }
            for (let i = 0; i < parts.length; i++) {
                let part = parts[i];
                if (i === (parts.length - 1)) {
                    ret += " " + part;
                }
                else if (part.length > 0) {
                    ret += part[0] + ".";
                }
            }
            return ret;
        },
        pageTitle() {
            return this.$store.state.pageTitle;
        },
        searchText: {
            get(){
                return this.$store.state.search.text;
            },
            set(text){
                if(text === undefined || text === null){
                    return;
                }
                router.appendQuery(this, { search: text });
                this.$store.commit('setSearchText', text);
            }
        },
        machineFilterActive(){
            return false;
        }
    },
    methods: {
        logout() {
            if(this.wasMounted){
                AuthState.logOut(this.$store);
            }
        },
        toggleSlide(){
            this.$store.commit('toggleSlide');
        },
    },
    components: {
        PageHome, SearchBar, SlideMenu
    },
}
</script>

<style>
.rlink-top-menu {
  color: #fff;
}

.rlink-top-menu:hover {
  color: #18bc9c;
  text-decoration: none;
}

.navbar-container {
  margin-left: 0px;
  margin-right: 0px;
  padding-right: 60px;
  width: 100%;
}

h2 {
  font-size: large;
  margin-top: 10.5px;
}

.main-container {
  margin-top: 60px;
  margin-left: 0px;
  margin-right: 50px;
  width: 100%;
  padding-left: 0;
  padding-right: 0;
}

.main-column {
  padding-right: 0;
  padding-left: 0;
  padding-bottom: 0;
  margin-top: 20px;
}

a {
  color: #001dff;
  cursor: pointer;
  font-weight: bold;
}

.page-title-div {
  padding-top: 10px;
  margin-left: 10px;
}

.page-title {
  color: white;
  text-shadow: black 2px 2px;
  font-size: x-large;
  border-top: solid;
  border-bottom: solid;
  border-width: 1px;
}

.main-search{
    margin-left: 20px;
    margin-top: 12px;
}

.right-nav-item {
    padding-left: 5px !important;
    padding-right: 5px !important;
}

.the-nav-container {
    padding-right: 20px;
}
</style>