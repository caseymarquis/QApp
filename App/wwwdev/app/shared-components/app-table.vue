<template>
    <div>
        <slot v-bind:sortHelper="sortHelper" v-bind:sortedItems="sortedItems">
        </slot>
    </div>
</template>
<script>

export default {
    props: ['items', 'filter', 'filterFn', 'defaultSortFn', 'defaultSortField', 'maxVisibleItems', 'defaultSortReverse'],
    created(){
        if(this.defaultSortReverse){
            this.sortHelper.sortDirection = -1;
        }
    },
    data(){
        return {
            sortHelper: {
                sortFn: null,
                sortGetFn: null,
                sortField: null,
                sortDirection: 1,
            }
        };
    },
    computed: {
        sortedItems(){
            let ret = this.items.slice();
            if(this.filter && this.filterFn){
                let trimmed = this.filter.trim().toUpperCase();
                if(trimmed !== ""){
                    ret = this.filterFn(trimmed, ret);
                }
            }
            
            let h = this.sortHelper;
            if(!h.sortField && !h.sortFn){
                if(this.defaultSortFn){
                    h.sortFn = this.defaultSortFn;
                }
                else if(this.defaultSortField){
                    h.sortField = this.defaultSortField;
                }
            }

            if(h.sortFn){
                ret.sort((a, b) => h.sortDirection*h.sortFn(a, b));
            }
            else if(h.sortGetFn){
                ret.sort((a, b) => h.sortDirection*this.sortByGetFn(a, b, h.sortGetFn));
            }
            else if(h.sortField){
                let field = h.sortField;
                ret.sort((a, b) => h.sortDirection*this.sortByFieldFn(a, b, field));
            }

            let wereVisible = ret.length;
            if(this.maxVisibleItems && this.maxVisibleItems > 0){
                ret = ret.slice(0, this.maxVisibleItems)
            }
            let nowVisible = ret.length;
            let isHidingExcessItems = (wereVisible !== nowVisible);

            //If you return early, then this won't be called:
            this.$emit('update', ret, isHidingExcessItems);
            return ret;
        },
    },
    methods: {
        sortByGetFn(a, b, getFn){
            a = getFn(a);
            b = getFn(b);
            if(!a && !b){
                return 0;
            }
            else if(!a){
                return 1;
            }
            else if(!b){
                return -1;
            }
            if(a.localeCompare){
                return a.localeCompare(b);
            }
            else{
                return (b - a);
            }
        },
        sortByFieldFn(a, b, field){
            let af = null;
            let bf = null;
            if(!field.includes(".")){
                if(!a && !b){
                    return 0;
                }
                else if(!a){
                    return 1;
                }
                else if(!b){
                    return -1;
                }

                af = a[field];
                bf = b[field];
            }
            else{
                let split = field.split(".");
                for(let i = 0; i < split.length; i++){
                    if(!a && !b){
                        return 0;
                    }
                    else if(!a){
                        return 1;
                    }
                    else if(!b){
                        return -1;
                    }

                    let currentField = split[i];
                    af = a[currentField];
                    bf = b[currentField];
                    if(!af || !bf){
                        break;
                    }
                    a = af;
                    b = bf;
                }
            }

            if(!af && !bf){
                return 0;
            }
            else if(!af){
                return -1;
            }
            else if(!bf){
                return 1;
            }

            if(af.localeCompare){
                return af.localeCompare(bf);
            }
            else{
                return (af - bf);
            }
        }
    }
}
</script>

<style scoped>
</style>