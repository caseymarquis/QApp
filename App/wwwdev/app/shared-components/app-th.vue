<template>
    <th v-on:click="sortByMe" style="cursor: pointer;">
        <div :class="{'pull-right': pullRight}">
            <slot></slot>
            <span v-if="sortDown" class="glyphicon glyphicon-chevron-down"></span>
            <span v-else-if="sortUp" class="glyphicon glyphicon-chevron-up"></span>
            <span v-else class="glyphicon glyphicon-sort"></span>
        </div>
    </th>
</template>

<script>

export default {
    props: ['sortHelper', 'sortFn', 'sortField', 'sortGetFn', 'pullRight', 'reverseSort'],
    data(){
        return {
        };
    },
    computed: {
        sortDown(){
            return this.sortDirectionIs(this.initialSortDirection);
        },
        sortUp(){
            return this.sortDirectionIs(-this.initialSortDirection);
        },
        initialSortDirection(){
            if(this.reverseSort){
                return -1;
            }
            return 1;
        }
    },
    methods: {
        sortDirectionIs(direction){
            let h = this.sortHelper;
            if(this.sortFn && h.sortFn){
                return (this.sortFn === h.sortFn) && (h.sortDirection === direction);
            }
            else if(this.sortGetFn && h.sortGetFn){
                return (this.sortGetFn === h.sortGetFn) && (h.sortDirection === direction);
            }
            else if(h.sortField){
                return (this.sortField === h.sortField) && (h.sortDirection === direction);
            }
            return false;
        },
        sortByMe(){
            let h = this.sortHelper;
            let isd = this.initialSortDirection;
            function resetH(){
                h.sortDirection = isd;
                h.sortFn = null;
                h.sortGetFn = null;
                h.sortField = null;
            }

            if(this.sortFn){
                if(h.sortFn === this.sortFn){
                    h.sortDirection *= -1;
                }
                else{
                    resetH();
                    h.sortFn = this.sortFn;
                }
            }
            else if(this.sortGetFn){
                if(h.sortGetFn === this.sortGetFn){
                    h.sortDirection *= -1;
                }
                else{
                    resetH();
                    h.sortGetFn = this.sortGetFn;
                }
            }
            else if(this.sortField){
                if(h.sortField === this.sortField){
                    h.sortDirection *= -1;
                }
                else{
                    resetH();
                    h.sortField = this.sortField;
                }
            }
        },
    }
}
</script>

<style scoped>
</style>