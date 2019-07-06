<template>
    <div class="form-inline" style="height: 100%;">
        <div class="input-group">
            <input class="form-control search-form-control" type="text" :placeholder="placeholder" v-model="searchText" v-on:keyup="raiseSearch">
            <button type="button" class="btn btn-info" v-on:click="raiseSearch" style="margin-left: -2px;">
                <fa-icon icon="search" size="lg" />
            </button>
        </div>
    </div>
</template>

<script>
export default {
    props: ["value", "placeholder"],
    data() {
        return {
            searchText: "",
            searchSequence: 0,
        }
    },
    created() {
        this.searchText = (this.value || "");
    },
    watch: {
        'value': 'updateInternalValue'
    },
    computed: {
    },
    methods: {
        raiseSearch() {
            this.searchSequence++;
            let searchSequenceWas = this.searchSequence;
            setTimeout(() => {
                if (this.searchSequence === searchSequenceWas) {
                    this.$emit("search", this.searchText);
                    this.$emit("input", this.searchText);
                }
            }, 400);
        },
        updateInternalValue(){
            this.searchText = this.value;
        }
    }
}
</script>

<style>
</style>