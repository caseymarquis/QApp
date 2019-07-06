<template>
    <div class="prop-set" v-show="shouldShow">
        <div class="col-main">
            <div class="form-inline d-flex the-form">
                <template v-if="type==='password'">
                    <div class="form-group a-form-group">
                            <label v-text="title + ':'"></label>
                            <template v-if="state === 'edit' || !passwordResetOnEdit">
                                <a v-if="!showHidden" style="color: blue">
                                    <span
                                        v-if="type==='password'"
                                        v-on:click="showHidden = true"
                                    >Click to Reset</span>
                                </a>
                            </template>
                            <button
                                class="help-btn btn btn-sm btn-info"
                                v-on:click="showHelp = !showHelp"
                            >
                                <fa-icon icon="question" />
                            </button>
                    </div>
                    <div class="form-group a-form-group" v-if="showHidden">
                            <password-edit
                                v-model="internalValue"
                                :allow-empty-password="allowEmptyPassword"
                                v-on:cancel="showHidden = !showHidden"
                                v-on:confirm="onConfirmPassword"
                            ></password-edit>
                    </div>
                </template>
                <template v-else-if="type==='clickToShow'">
                    <div class="form-group a-form-group">
                            <button
                                class="help-btn btn btn-sm btn-info"
                                v-on:click="showHelp = !showHelp"
                            >
                                <fa-icon icon="question" />
                            </button>
                            <label v-text="title + ':'"></label>
                            <a>
                                <span
                                    v-if="showHidden"
                                    v-on:click="showHidden = !showHidden"
                                >Click to Hide</span>
                                <span v-else v-on:click="showHidden = !showHidden">Click to Show</span>
                            </a>
                    </div>
                    <div class="form-group a-form-group" v-if="showHidden">
                        <div class="col-12">
                            <p v-text="getDisplayText"></p>
                        </div>
                    </div>
                </template>
                <template v-else>
                    <div class="form-group a-form-group">
                            <label v-text="title + ':'"></label>
                            <span
                                v-if="state !== 'edit' && type !== 'custom'"
                                v-text="getDisplayText"
                            ></span>

                            <input
                                v-if="state === 'edit' && type==='bool'"
                                type="checkbox"
                                class="settings-checkbox"
                                style="margin-left: 10px; margin-right: 10px;"
                                v-model="internalValue"
                            />

                            <button
                                class="help-btn btn btn-sm btn-info"
                                v-on:click="showHelp = !showHelp"
                            >
                                <fa-icon icon="question" />
                            </button>

                            <slot name="display" v-if="state !== 'edit' && type === 'custom'"></slot>
                            <button
                                v-if="highlightText"
                                v-on:click="showHighlightDescription = !showHighlightDescription"
                                class="btn btn-sm btn-info btn-highlight"
                            >
                                <fa-icon class="exclamation"></fa-icon>
                            </button>
                    </div>
                    <div class="form-group a-form-group" v-if="state === 'edit' && type !== 'bool'">
                            <select
                                v-if="type==='enum'"
                                type="checkbox"
                                class="form-control"
                                v-model="internalValue"
                            >
                                <option v-for="option in options" v-text="option" :key="option"></option>
                            </select>
                            <input
                                v-else-if="type==='int'"
                                type="number"
                                step="1"
                                class="form-control"
                                v-model="internalValue"
                            />
                            <input
                                v-else-if="type==='nat'"
                                type="number"
                                min="0"
                                step="1"
                                class="form-control"
                                v-model="internalValue"
                            />
                            <input
                                v-else-if="type==='float'"
                                type="number"
                                class="form-control"
                                v-model="internalValue"
                            />
                            <input
                                v-else-if="type==='clickToShow'"
                                type="password"
                                class="form-control wide-input"
                                v-model="internalValue"
                            />
                            <input
                                v-else-if="type==='password'"
                                type="password"
                                class="form-control wide-input"
                                v-model="internalValue"
                            />
                            <date-time-picker v-else-if="type==='date'" v-model="asmoment"></date-time-picker>
                            <input
                                v-else-if="type!=='custom'"
                                type="text"
                                class="form-control wide-input"
                                v-model="internalValue"
                            />
                            <slot name="edit" v-else></slot>
                    </div>
                </template>
            </div>
        </div>
        <div class="card bg-light help-well" v-if="shouldShow && showHelp">
            <slot></slot>
        </div>
        <div class="card bg-light help-well" v-if="shouldShow && showHighlightDescription">
            <p v-text="highlightText"></p>
        </div>
    </div>
</template>

<script>
import PasswordEdit from "./password-edit.vue";
import DateTimePicker from "./date-time-picker.vue";
import moment from "moment";

//https://stackoverflow.com/a/40915857/1109665 //Implementing your own v-model. :D
export default {
    props: [
        "value",
        "title",
        "state",
        "filter",
        "type",
        "options",
        "allowEmptyPassword",
        "passwordResetOnEdit",
        "highlight"
    ],
    data() {
        return {
            showHelp: false,
            internalValue: null,
            showHidden: false,
            showHighlightDescription: false
        };
    },
    created() {
        this.internalValue = this.value;
    },
    watch: {
        value: function() {
            this.internalValue = this.value;
        },
        internalValue: function() {
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
            } else if (this.type === "date") {
                if (v === null) {
                    return "N/A";
                }
                if (!moment.isMoment(v)) {
                    v = new moment(v);
                }
                return v.format("MM/DD/YY");
            }
            return v;
        },
        highlightText() {
            if (this.highlight && this.highlight.filter) {
                let getMatch = () => {
                    for (let i = 0; i < this.highlight.length; i++) {
                        let h = this.highlight[i];
                        if (h.value && h.value.filter) {
                            let isMatch = h.value.some(
                                x => x === this.internalValue
                            );
                            if (h.inverse && !isMatch) {
                                return h;
                            } else if (!h.inverse && isMatch) {
                                return h;
                            }
                        } else if (
                            h.inverse &&
                            h.value !== this.internalValue
                        ) {
                            return h;
                        } else if (
                            !h.inverse &&
                            h.value === this.internalValue
                        ) {
                            return h;
                        }
                    }
                };
                let match = getMatch();
                if (match) {
                    return match.text || "Not the standard value.";
                }
            }
            return undefined;
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
            get() {
                if (this.internalValue == null) {
                    return null;
                }
                if (moment.isMoment(this.internalValue)) {
                    return this.internalValue;
                } else {
                    return new moment(this.internalValue);
                }
            },
            set(value) {
                if (moment.isMoment(this.internalValue)) {
                    this.internalValue = value;
                } else {
                    this.internalValue = value.toISOString();
                }
            }
        }
    },
    methods: {
        onConfirmPassword(newPassword) {
            this.showHidden = false;
            this.internalValue = newPassword;
            this.$emit("confirm", newPassword);
        }
    },
    components: {
        PasswordEdit,
        DateTimePicker
    }
};
</script>

<style lang="scss" scoped>
.prop-set {
    padding-top: 5px;
    border-right: solid;
    border-left: solid;
    border-bottom: solid;
    border-width: 1px;
}

.col-main{
    display: flex;
    flex-direction: column;
    padding-left: 10px;
}

label {
    font-weight: bold;
    margin-right: 5px;
}

.help-well {
    margin-top: 5px;
    padding-left: 10px;
}

.help-btn {
    $y: 2px;
    $x: 5px;
    margin-right: 5px;
    margin-left: auto;
    margin-bottom: 5px;
    padding: $y $x $y $x;
}

.wide-input {
    min-width: 50%;
}

.btn-highlight {
    padding: 0 2px 0 2px;
}

.the-form{
    display: flex;
    flex-direction: column;
    justify-content: space-between;
}

.a-form-group{
    display: flex;
    flex-direction: row;
    justify-content: flex-start;
    width: 100%;
}
</style>