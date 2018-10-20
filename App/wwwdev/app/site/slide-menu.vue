<template>
    <div class="menu-div"
        :class="{enterScreenLeft: visible, exitScreenLeft: (!visible && hasEntered) }">
        <div class="container menu-container">
            <div class="panel panel-primary menuPanel">
                <div class="panel-heading">
                    <a v-on:click="toggleVisible" style="color: white; float: right;">
                        X
                    </a>
                    &nbsp;&nbsp;
                </div>
                <div class="panel-body the-panel-body">
                    <div class="row">
                        <div class="col-xs-12">
                        </div>
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import store from '../js/store.js';
import router from '../js/router.js';

import SearchBar from '../shared-components/search-bar.vue';

export default {
    props: [],
    created(){
    },
    data() {
        return {
        };
    },
    watch: {
        '$route': 'updateRouteFromStore',
    },
    computed: {
        visible() {
            return store.state.slide.visible;
        },
        hasEntered(){
            return store.state.slide.hasEntered;
        },
    },
    methods: {
        toggleVisible() {
            store.commit('toggleSlide');
        },
    },
    components: {
        SearchBar,
    },
}
</script>

<style scoped>
.menu-div {
  width: 50%;
  position: absolute;
  z-index: 9999;
  padding: 0 0 0 0;
  margin-top: 25px;
  margin-left: -100%;
  -moz-box-sizing: border-box;
  -webkit-box-sizing: border-box;
  box-sizing: border-box;
  overflow-x: hidden;
}
.menu-container {
  width: 100%;
  min-width: 100%;
  max-width: 100%;
  height: 98%;
  padding: 0 0 0 0;
}

.menu-left-border {
  border-left: solid;
  border-left-color: white;
}

.the-panel-body{
    overflow-y:scroll;
    height: 70vh;
}

.enterScreenLeft {
  animation: enterScreenLeftKf 0.5s forwards;
}
.exitScreenLeft {
  animation: exitScreenLeftKf 0.5s forwards;
}
@keyframes exitScreenLeftKf {
  from {
    margin-left: 0%;
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
    margin-left: 0%;
  }
}
</style>