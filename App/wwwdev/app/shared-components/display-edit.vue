<template>
    <div class="card" style="margin-bottom: 20px;">
        <div class="card-header the-header bg-info text-white">
            <slot name="title">
                <span class="the-title" v-text="name"></span>
            </slot>
            <div class="float-right">
                <button
                    v-if="state === 'awaitEdit' && !disableEdit"
                    class="btn btn-warning"
                    style="margin-left: 5px; padding-top: 3px; padding-bottom: 3px;"
                    v-on:click="$emit('edit')"
                >Edit</button>
                <button
                    v-if="allowDelete === true"
                    class="btn btn-danger"
                    style="padding-top: 3px; padding-bottom: 3px;"
                    v-on:click="$emit('delete')"
                >
                    <fa-icon icon="times" />
                </button>
            </div>
        </div>
        <div
            class="card-body display-edit-body"
            v-if="!(hideBody === true && state === 'awaitEdit')"
        >
            <div v-if="state === 'load'">Loading</div>
            <div v-else-if="state === 'save'">Saving</div>
            <div style="padding-top: 5px; padding-bottom: 5px;" v-else-if="state === 'edit'">
                <button class="btn btn-info" v-on:click="$emit('discard')">Discard</button>
                <button class="btn btn-success float-right" v-on:click="$emit('save')">Save</button>
            </div>
            <slot></slot>
        </div>
    </div>
</template>

<script>
export default {
    props: ["name", "state", "hideBody", "allowEdit", "allowDelete"],
    data() {
        return {};
    },
    computed: {
        disableEdit() {
            return this.allowEdit === false;
        }
    }
};
</script>

<style lang="scss" scoped>
.display-edit-body {
    padding: 0 0 0 0;
    overflow-x: hidden;
}

.the-header{
    $header-padding: 5px;
    padding-top: $header-padding;
    padding-bottom: $header-padding;
}

.the-title{
    font-size: large;
}
</style>