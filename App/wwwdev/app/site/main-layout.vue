<template>
    <div>
        <nav id="navbar-main" class="navbar navbar-main navbar-expand-lg navbar-dark bg-primary">
            <router-link to="/" class="navbar-brand">
                Logo
                <!--img :src="imgLogo" class="center-block visible-lg" style="height: 70px; margin-left: 0;" /-->
                <!--img :src="imgLogo" class="center-block visible-md visible-sm" style="max-height: 35px; margin-left: 0; margin-top: 15px;" /-->
            </router-link>
            <button
                class="navbar-toggler"
                type="button"
                data-toggle="collapse"
                data-target="#navbarSupportedContent"
                aria-controls="navbarSupportedContent"
                aria-expanded="false"
                aria-label="Toggle navigation"
            >
                <span class="navbar-toggler-icon"></span>
            </button>
            <div class="collapse navbar-collapse" id="navbarSupportedContent">
                <ul class="navbar-nav mr-auto">
                    <li class="nav-item active">
                        <h2 class="page-title visible-lg" v-text="pageTitle"></h2>
                    </li>
                    <li v-if="loggedIn" class="nav-item active">
                        <button class="btn btn-primary slide-menu-btn" v-on:click="toggleSlide">
                            <fa-icon icon="bars" size="lg" />
                        </button>
                    </li>
                    <li v-if="loggedIn && $store.state.search.show" class="nav-item active">
                        <search-bar
                            v-model="searchText"
                            class="main-search"
                            placeholder="Search..."
                        ></search-bar>
                    </li>
                </ul>
                <ul v-if="loggedIn" class="navbar-nav ml-auto">
                    <li class="nav-item active">
                        <router-link :to="'/user/' + $store.state.user.id" class="nav-link">
                            <span v-text="userFirstName"></span>
                        </router-link>
                    </li>
                    <li class="nav-item active">
                        <a class="nav-link" v-on:click="logout">Log Out</a>
                    </li>
                    <li class="nav-item active">
                        <router-link to="/" class="nav-link">Home</router-link>
                    </li>
                </ul>
            </div>
        </nav>

        <div class="container-fluid container-fluid-main" :style="containerMainStyle">
            <slide-menu></slide-menu>
            <page-home style="height: 100%;" v-if="!loggedIn"></page-home>
            <router-view style="height: 100%;" v-else></router-view>
        </div>
    </div>
</template>

<script>
import PageHome from "../page-home/page-home.vue";
import SearchBar from "../shared-components/search-bar.vue";
import SlideMenu from "./slide-menu.vue";
//import imgLogo from "../img/logo.png";

import AuthState from "../js/AuthState.js";
import SetupPage from "../js/SetupPage.js";
import router from "../js/router.js";

export default {
    data() {
        return {
            //imgLogo,
            wasMounted: false
        };
    },
    mounted() {
        this.wasMounted = true;
    },
    created() {
        this.updateScreenHeight();
        setTimeout(() => {
            this.updateScreenHeight();
        }, 100);
        setInterval(() => {
            this.updateScreenHeight();
        }, 500);
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
                if (i === parts.length - 1) {
                    ret += " " + part;
                } else if (part.length > 0) {
                    ret += part[0] + ".";
                }
            }
            return ret;
        },
        pageTitle() {
            return this.$store.state.pageTitle;
        },
        searchText: {
            get() {
                return this.$store.state.search.text;
            },
            set(text) {
                if (text === undefined || text === null) {
                    return;
                }
                router.appendQuery(this, { search: text });
                this.$store.commit("setSearchText", text);
            }
        },
        containerMainStyle(){
            return this.$store.state.pageFluid? { height: `${this.$store.state.screenHeight}px` } : {};
        }
    },
    methods: {
        logout() {
            if (this.wasMounted) {
                AuthState.logOut(this.$store);
            }
        },
        toggleSlide() {
            this.$store.commit("toggleSlide");
        },
        updateScreenHeight() {
            var navEl = document.getElementById('navbar-main');
            if(navEl){
                let newVal = window.innerHeight - navEl.clientHeight;
                if(newVal !== this.$store.state.screenHeight){
                    this.$store.commit('setScreenHeight', newVal);
                }
            }
        }
    },
    components: {
        PageHome,
        SearchBar,
        SlideMenu
    }
};
</script>

<style lang="scss">
@import "globals.scss";

.navbar-main{
    min-height: $navbar-main-height;
}

.bg-primary {
    .navbar-nav .active > .nav-link {
        color: #fff !important;
        font-size: large;
        font-weight: normal;
    }
    .navbar-nav .active > .nav-link:hover {
        color: #18bc9c !important;
    }
}

$brand-padding: 15px;
.navbar-brand {
    padding-left: $brand-padding;
    padding-right: $brand-padding;
    font-size: large;
}

a {
    color: blue;
    cursor: pointer;
    font-weight: bold;
}

.page-title {
    color: white;
    text-shadow: black 2px 2px;
    font-size: x-large;
    border-top: solid;
    border-bottom: solid;
    border-width: 1px;
    margin-top: 10.5px;
}

$slide-menu-btn-margin: 10px;
.slide-menu-btn{
    height: 100%;
    margin-left: $slide-menu-btn-margin;
    margin-right: $slide-menu-btn-margin;
}

.main-search {
    height: 100%;
}

.container-fluid{
    padding-left: $container-fluid-padding;
    padding-right: $container-fluid-padding;
}
</style>