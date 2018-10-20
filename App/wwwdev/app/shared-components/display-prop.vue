<template>
    <div>
        <div class="row prop-set" v-show="shouldShow">
            <div class="col-xs-12">
                <div class="form-inline">
                    <div v-if="type==='password'">
                        <div class="row">
                            <div class="col-xs-12">
                                <button class="help-btn btn btn-xs btn-info pull-right" v-on:click="showHelp = !showHelp"><span class="glyphicon glyphicon-question-sign"></span></button>
                                <label v-text="title + ':'"></label>
                                <a v-if="!showHidden">
                                    <span v-if="type==='password'" v-on:click="showHidden = true">Click to Reset</span>
                                </a>
                            </div>
                        </div>
                        <div class="row" v-if="showHidden">
                            <div class="col-xs-12">
                                <password-edit v-model="internalValue" :allow-empty-password="allowEmptyPassword" v-on:cancel="showHidden = !showHidden" v-on:confirm="onConfirmPassword"></password-edit>
                            </div>
                        </div>
                    </div>
                    <div v-else-if="type==='clickToShow'">
                        <div class="row">
                            <div class="col-xs-12">
                                <button class="help-btn btn btn-xs btn-info pull-right" v-on:click="showHelp = !showHelp"><span class="glyphicon glyphicon-question-sign"></span></button>
                                <label v-text="title + ':'"></label>
                                <a>
                                    <span v-if="showHidden" v-on:click="showHidden = !showHidden">Click to Hide</span>
                                    <span v-else            v-on:click="showHidden = !showHidden">Click to Show</span>
                                </a>
                            </div>
                        </div>
                        <div class="row" v-if="showHidden">
                            <div class="col-xs-12">
                                <p v-text="getDisplayText"></p>
                            </div>
                        </div>
                    </div>
                    <div v-else>
                        <div class="row">
                            <div class="col-xs-12">
                                <label v-text="title + ':'"></label>
                                <span v-if="state !== 'edit'" v-text="getDisplayText"></span>

                                <input v-if="state === 'edit' && type==='bool'" type="checkbox" class="settings-checkbox" style="margin-left: 10px; margin-right: 10px;" v-model="internalValue" />

                                <button class="help-btn btn btn-xs btn-info pull-right" v-on:click="showHelp = !showHelp"><span class="glyphicon glyphicon-question-sign"></span></button>
                            </div>
                        </div>
                        <div class="row" v-if="state === 'edit' && type !== 'bool'">
                            <div class="col-xs-12">
                                <select v-if="type==='enum'" type="checkbox" class="form-control" v-model="internalValue">
                                    <option v-for="option in options" v-text="option" :key="option"></option>
                                </select>
                                <input v-else-if="type==='int'" type="number" step="1" class="form-control" v-model="internalValue" />
                                <input v-else-if="type==='nat'" type="number" min="0" step="1" class="form-control" v-model="internalValue" />
                                <input v-else-if="type==='float'" type="number" class="form-control" v-model="internalValue" />
                                <input v-else-if="type==='clickToShow'" type="password" class="form-control" v-model="internalValue"/>
                                <input v-else-if="type==='password'" type="password" class="form-control" v-model="internalValue"/>
                                <date-time-picker v-else-if="type==='date'" v-model="asmoment"></date-time-picker>
                                <input v-else type="text" class="form-control" v-model="internalValue"/>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </div>
        <div class="well well-sm help-well" v-if="showHelp">
            <slot></slot>
        </div>
    </div>
</template>

<script>
import PasswordEdit from "./password-edit.vue";
import DateTimePicker from "./date-time-picker.vue";
import moment from "moment";

//https://stackoverflow.com/a/40915857/1109665 //Implementing your own v-model. :D
export default {
    props: ["value", "title", "state", "filter", "type", "options", "allowEmptyPassword"],
    data() {
        return {
            showHelp: false,
            internalValue: null,
            showHidden: false
        };
    },
    created() {
        this.internalValue = this.value;
    },
    watch: {
        value: function () {
            this.internalValue = this.value;
        },
        internalValue: function () {
            if (this.internalValue !== undefined) {
                this.$emit("input", this.internalValue);
            }
        }
    },
    computed: {
        getDisplayText() {
            let v = this.internalValue;
            let t = typeof v;
            if (typeof v === "boolean") {
                return v ? "Yes" : "No";
            }
            else if(this.type === "date"){
                if(v === null){
                    return "N/A"
                }
                if(!moment.isMoment(v)){
                    v = new moment(v);
                }
                return v.format('MM/DD/YY');
            }
            return v;
        },
        shouldShow() {
            let f = this.filter;
            let t = this.title;
            if (f === null || f === undefined) {
                return true;
            }
            if (f === "") {
                return true;
            }
            f = f.trim();
            if (f === "") {
                return true;
            }
            f = f.toUpperCase();
            t = t.trim().toUpperCase();
            return t.includes(f);
        },
        asmoment: {
            get(){
                if(this.internalValue == null){
                    return null;
                }
                if(moment.isMoment(this.internalValue)){
                    return this.internalValue;
                }
                else{
                    return new moment(this.internalValue);
                }
            },
            set(value){
                if(moment.isMoment(this.internalValue)){
                    this.internalValue = value;
                }
                else{
                    this.internalValue = value.toISOString();
                }
            },
        }
    },
    methods: {
        onConfirmPassword(newPassword) {
            this.showHidden = false;
            this.internalValue = newPassword;
        }
    },
    components: {
        PasswordEdit, DateTimePicker
    }
};
</script>

<style scoped>
.prop-set {
  padding-left: 10px;
  padding-top: 5px;
  margin-top: 3px;
  border-top: solid;
  border-width: 2px;
}

.help-well {
  margin-top: 5px;
  margin-bottom: 0;
}

.help-btn {
  margin-right: 5px;
}
</style>