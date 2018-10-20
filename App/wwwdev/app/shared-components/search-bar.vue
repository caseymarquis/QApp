<template>
    <div>
        <div class="form-inline">
            <div class="input-group">
                <input class="form-control search-form-control" type="text" :placeholder="placeholder" v-model="searchText" v-on:keyup="raiseSearch">
                <span class="input-group-btn">
                    <button class="btn btn-default" v-on:click="raiseSearch">
                        <span class="glyphicon glyphicon-search"></span>
                    </button>
                </span>
            </div>
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