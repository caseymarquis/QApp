<template>
    <div class="row">
        <div class="col-10" style="padding-right: 0;">
            <input v-model="text" :id="`${idBase}in`" v-on:keyup="onKeyUp" v-on:input="onInput" v-on:click="onClick" class="form-control the-input" :list="`${idBase}list`" :name="name" :placeholder="placeholder" />
            <datalist :id="`${idBase}list`">
                <slot>
                </slot>
            </datalist>
        </div>
        <div class="col-2" style="padding-left: 0; height: 100%;">
            <button class="btn btn-primary" style="object-fit: fill;" v-on:click="clear">
                <fa-icon icon="times" />
            </button>
        </div>
    </div>
</template>

<script>
import datalist from 'datalist-polyfill';

let listCounter = 0;

export default {
    props: ["value", "name", "placeholder"],
    created(){
        listCounter++;
        this.idBase = 'dlist' + listCounter;
        this.text = this.value;
    },
    data() {
        return {
            idBase: "",
            internalValue: "",
            text: "",
        }
    },
    watch: {
        'value': 'updateText',
    },
    methods: {
        onKeyUp(ev){
            if(ev.keyCode === 13){
                this.$emit('enter');
                let el = document.getElementById(`${this.idBase}in`);
                if(el){
                    el.blur();
                    el.focus();
                }
            }
        },
        updateText(){
            this.text = this.value;
        },
        onInput(ev){
            if(this.text === "" || this.text){
                this.$emit('input', this.text.trim());
            }
        },
        clear(){
            this.text = "";
            this.$emit('input', "");
            let el = document.getElementById(`${this.idBase}in`);
            if(el){
                el.focus();
            }
        },
        onClick(){
            if(this.groupText !== ""){
                this.clear();
            }
        },
    }
}
</script>

<style scoped>
    .stretch{
        width: 100%;
        height: 100%;
    }
</style>