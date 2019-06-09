<template>
    <div class="row editor-first-row">
        <div class="col-lg-1">
        </div>
        <div class="col-lg-10">
            <div class="row">
                <div class="col-sm-6">
                    <slot name="header">
                    </slot>
                </div>
                <div class="col-sm-6">
                    <button v-if="showCreateNew" class="btn btn-success pull-right" v-on:click="$emit('createNew')">
                        <span class="glyphicon glyphicon-plus"></span>
                        Create New <span v-text="itemName"></span>
                    </button>
                    <search-bar v-if="showSearch2" class="pull-right" placeholder="Search Properties..." v-on:search="onSearch2"></search-bar> 
                </div>
            </div>
            <div class="row pm-edit-row">
                <div v-for="item in items" v-show="(filter.trim() === '') || shouldShowItem(item)" :key="item.id">
                    <slot v-bind:item="item" v-bind:filter="filter2">
                    </slot>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import SearchBar from "./search-bar.vue";
import SetupPage from "../js/SetupPage.js";

export default {
    props: ['items', 'itemName', 'showItem', 'showCreateNew', 'showSearch2'],
    data() {
        return {
            filter2: "",
        };
    },
    created() {
    },
    computed: {
        filter(){
            return this.$store.state.search.text.trim().toUpperCase();
        }
    },
    watch: {
        'filter': 'onSearch',
    },
    methods: {
        onSearch() {
            if(this.filter !== undefined){
                this.$emit("search", this.filter.trim().toUpperCase());
            }
        },
        onSearch2(text) {
            this.filter2 = text;
            this.$emit("search2", text.toUpperCase().trim());
        },
        shouldShowItem(item) {
            if (typeof (this.showItem) === "function") {
                return this.showItem(this.filter, item)
            }
            else if (item !== null && item !== undefined) {
                if (typeof (item.name) === 'string') {
                    return item.name.toUpperCase().includes(this.filter);
                }
            }
            return true;
        },
    },
    components: {
        SearchBar,
    }
}
</script>

<style scoped>
.editor-first-row {
  margin-top: 10px;
}
.pm-edit-row {
  margin-top: 10px;
}
</style>