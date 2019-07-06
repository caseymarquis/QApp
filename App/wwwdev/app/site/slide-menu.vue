<template>
    <div
        class="menu-div"
        :class="{enterScreenLeft: visible, exitScreenLeft: (!visible && hasEntered) }"
        :style="menuDivStyleText"
    >
        <div class="container-fluid menu-container" style="height: 99.7%;">
            <div class="card menuPanel" style="height: 100%;">
                <div class="card-header bg-primary title-header">
                    <a v-on:click="toggleVisible" style="color: white; float: left;">
                        <fa-icon icon="arrow-left" />
                    </a>
                    <a v-on:click="toggleVisible" style="color: white; float: right;">X</a>
                    &nbsp;&nbsp;
                </div>
                <div class="panel-body the-panel-body">
                    <div class="row">
                        <div class="col-12"></div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import store from "../js/store.js";
import router from "../js/router.js";

import SearchBar from "../shared-components/search-bar.vue";
import { setInterval } from "timers";

export default {
    props: [],
    created() {
    },
    data() {
        return {
        };
    },
    watch: {},
    computed: {
        visible() {
            return store.state.slide.visible;
        },
        hasEntered() {
            return store.state.slide.hasEntered;
        },
        menuDivStyleText() {
            return `height: ${this.$store.state.screenHeight}px`;
        }
    },
    methods: {
        toggleVisible() {
            store.commit("toggleSlide");
        }
    },
    components: {
        SearchBar
    }
};
</script>

<style lang="scss" scoped>
@import "globals.scss";

.menu-div {
    width: 50%;
    position: absolute;
    z-index: 9999;
    padding: 0 0 0 0;
    margin-top: 0;
    margin-left: -100%;
    -moz-box-sizing: border-box;
    -webkit-box-sizing: border-box;
    box-sizing: border-box;
    overflow-x: hidden;
}
.menu-container {
    padding-left: 0;
    padding-right: 0;
}

.the-panel-body {
    overflow-y: scroll;
}

$animation-time: 0.5s;
.enterScreenLeft {
    animation: enterScreenLeftKf $animation-time forwards;
}
.exitScreenLeft {
    animation: exitScreenLeftKf $animation-time forwards;
}
@keyframes exitScreenLeftKf {
    from {
        margin-left: 0%;
        margin-left: -$container-fluid-padding;
    }
    to {
        margin-left: -100%;
    }
}
@keyframes enterScreenLeftKf {
    from {
        margin-left: -100%;
    }
    to {
        margin-left: -$container-fluid-padding;
    }
}
</style>