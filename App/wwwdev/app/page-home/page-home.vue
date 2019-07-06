<template>
    <div>
        <div class="row hidden-md-down" style="border-bottom:solid; height: 10%;">
            <div class="col-6 d-flex" style="flex-direction: column-reverse;">
                <h1 class="app-title">An App</h1>
            </div>
            <div class="col-6 d-flex" style="flex-direction: column-reverse;">
                    <h3 class="directory-title">
                        <span v-if="!loggedIn">Log In</span>
                        <span v-else>What would you like to do?</span>
                    </h3>
            </div>
        </div>
        <div class="row" style="height: 90%;">
            <div class="col-6 hidden-xs-down" style="border-right:solid;">
                <div class="row home-market-row">
                    <div class="col-12">
                        <h2 class="home-market-title">
                            Welcome to some app.
                        </h2>
                        <p
                            class="home-market-text"
                        >This is where some stuff about the app could be written.</p>
                    </div>
                </div>
                <div class="row home-market-row">
                    <div class="col-12">
                        <h2 class="home-market-title">
                            More stuff!
                        </h2>
                        <p
                            class="home-market-text"
                        >Even more could be written here! Untold troves of information!</p>
                    </div>
                </div>
                <div class="row home-market-row">
                    <div class="col-12">
                        <h2 class="home-market-title">
                            The rule of three.
                        </h2>
                        <p class="home-market-text">
                            The rule of three is a writing principle that suggests that a trio of events or characters is more humorous,
                            satisfying, or effective than other numbers in execution of the story and engaging the reader.
                            I didn't make the rules.
                        </p>
                    </div>
                </div>
            </div>
            <div class="col-sm-6 right-home-column">
                <app-login v-if="!loggedIn"></app-login>
                <app-directory v-else></app-directory>
            </div>
        </div>
    </div>
</template>

<script>
import AppLogin from "../shared-components/app-login.vue";
import AppDirectory from "./app-directory.vue";

//import imgA from "../img/a.png";
//import imgB from "../img/b.jpg";
//import imgC from "../img/c.png";
//import imgLogo from "../img/logo.png";
import SetupPage from "../js/SetupPage.js";

export default {
    mounted() {
        SetupPage.title("").fluid();
    },
    data() {
        return {
            //imgA,
            //imgB,
            //imgC,
            //imgLogo
        };
    },
    computed: {
        loggedIn() {
            return this.$store.state.loggedIn;
        }
    },
    components: {
        AppLogin,
        AppDirectory
    }
};
</script>

<style>
.app-title{
    text-shadow: #aaa 2px 2px 2px;
    text-align: center;
}

.directory-title {
    text-shadow: #aaa 2px 2px 2px;
    text-align: center;
}

.home-market-row{
    border-bottom: solid;
    padding-top: 10px;
}

.home-market-title {
    color: black;
    font-size: x-large;
    text-align: center;
    text-shadow: #aaa 1px 1px 1px;
}

.home-market-text {
    text-align: center;
}
</style>