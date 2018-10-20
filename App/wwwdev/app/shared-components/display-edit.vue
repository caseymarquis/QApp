<template>
    <div>
        <div class="panel panel-info">
            <div class="panel-heading">
                <div class="row">
                    <div class="col-xs-8">
                        <slot name="title">
                            <h3 class="panel-title" style="margin-top: 7px;" v-text="name"></h3>
                        </slot>
                    </div>
                    <div class="col-xs-4">
                        <button v-if="state === 'awaitEdit' && !disableEdit" class="btn btn-warning pull-right" style="padding-top: 3px; padding-bottom: 3px;" v-on:click="$emit('edit')">Edit</button>
                    </div>
                </div>
            </div>
            <div class="panel-body" v-if="!(hideBody === true && state === 'awaitEdit')">
                <div v-if="state === 'load'">
                    Loading
                </div>
                <div v-else-if="state === 'save'">
                    Saving
                </div>
                <div v-else-if="state === 'edit'">
                    <button class="btn btn-info" v-on:click="$emit('discard')">Discard</button>
                    <button class="btn btn-success pull-right" v-on:click="$emit('save')">Save</button>
                </div>
                <slot>
                </slot>
            </div>
        </div>
    </div>
</template>

<script>
export default {
    props: ["name", "state", "hideBody", "allowEdit"],
    data() {
        return {
        }
    },
    computed: {
        disableEdit() {
            return (this.allowEdit === false);
        }
    }
}
</script>

<style scoped>
</style>