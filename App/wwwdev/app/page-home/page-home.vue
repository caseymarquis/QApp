<template>
    <div>
        <div class="row hidden-xs top-row" style="border-bottom:solid">
            <div class="col-xs-12 home-jumbo">
                <div class="col-sm-6 hidden-xs title-column">
                    <div class="row title-row">
                        <div class="jumbo-title-div">
                            <div class="hidden-sm">
                                <!--img :src="imgLogo" class="center-block" style="width: 70%; margin-left: 15%;" /-->
                            </div>
                        </div>
                    </div>
                </div>
                <div class="col-xs-6">
                    <div class="">
                        <h2>&nbsp;</h2>
                        <h3 class="directory-title">
                            <span v-if="!loggedIn">Log In</span>
                            <span v-else>What would you like to do?</span>
                        </h3>
                    </div>
                </div>
            </div>
        </div>
        <div class="col-sm-6 hidden-xs" style="border-right:solid">
            <div class="row" style="border-bottom:solid">
                <div class="col-md-12">
                    <h2 class="home-market-title">Welcome to some app. <span class="glyphicon glyphicon-glass"></span></h2>
                    <p class="home-market-text">
                        This is where some stuff about the app could be written.
                    </p>
                </div>
                <div class="col-md-6 hidden-sm hidden-xs hidden">
                    <!--img :src="imgTablet" class="center-block tablet-img" style="width: 222px; height: 166px;" align="middle"/-->
                </div>
            </div>
            <div class="row" style="border-bottom:solid">
                <div class="col-md-12">
                    <h2 class="home-market-title">More stuff! <span class="glyphicon glyphicon-time"></span></h2>
                    <p class="home-market-text">
                        Even more could be written here! Untold troves of information!
                    </p>
                </div>
                <div class="col-md-6 hidden-sm hidden-xs hidden">
                    <!--img :src="imgSched" class="center-block" style="width: 100%" /-->
                </div>
            </div>
            <div class="row" style="">
                <div class="col-md-12">
                    <h2 class="home-market-title">The rule of three. <span class="glyphicon glyphicon-trash"></span></h2>
                    <p class="home-market-text">
                        The rule of three is a writing principle that suggests that a trio of events or characters is more humorous,
                        satisfying, or effective than other numbers in execution of the story and engaging the reader.
                        I didn't make the rules.
                    </p>
                </div>
                <div class="col-md-6 hidden-sm hidden-xs hidden">
                    <!--img :src="imgBarChart" class="center-block bar-chart-img" style="width: 222px; height: 166px;"/-->
                </div>
            </div>
        </div>
        <div class="col-sm-6 right-home-column">
            <app-login v-if="!loggedIn"></app-login>
            <app-directory v-else></app-directory>
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
        SetupPage.title("");
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
        },
    },
    components: {
        AppLogin,
        AppDirectory
    }
}
</script>

<style>
.system-column {
  padding: 0 0 0 0;
  margin: 0 0 0 0;
}

.main-log-view {
  max-height: 90vh;
  min-height: 90vh;
}

.log-panel {
  margin-bottom: 0;
}

.home-jumbo {
  margin-bottom: 0;
  padding-left: 0 !important;
  padding-right: 0 !important;
  padding-top: 15px !important;
  padding-bottom: 0px !important;
}

.home-market-title {
  color: black;
  font-size: x-large;
  text-align: center;
  text-shadow: #aaa 2px 2px 2px;
}

.home-market-text {
    text-align: center;
}

.jumbo-title-div {
  margin-bottom: 0;
  text-align: center;
}

.title-column {
  padding-left: 0;
}

.jumbo-header {
  font-size: 4em;
  color: #1772af;
  text-shadow: #aaa 2px 2px 2px;
}

.smaller-header {
  color: #1772af;
  text-shadow: #aaa 1px 1px 1px;
}

.jumbo-sub-title {
  text-shadow: #aaa 2px 2px 2px;
}

.title-row {
  padding-bottom: 5px;
}

.directory-title {
  text-shadow: #aaa 2px 2px 2px;
  margin-top: 55px;
  margin-left: -50px;
  text-align: center;
}

.right-home-column {
  overflow-y: scroll;
  max-height: 75vh;
  min-height: 75vh;
}

.top-row {
  margin-left: 0;
  margin-right: 0;
}
</style>