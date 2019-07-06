<template>
    <div>
        <div class="form-group my-form-group">
            <div class='input-group' :id='myId'>
                <input type='text' class="form-control" />
                <span class="input-group-addon">
                    <fa-icon icon="calendar-alt"/>
                </span>
            </div>
        </div>
    </div>
</template>

<script>
import moment from "moment";
import ebd from "eonasdan-bootstrap-datetimepicker";
import $ from "jquery";

let id = 0;

export default {
    props: ["value"],
    data() {
        return {
            myId: "",
        }
    },
    created() {
        this.myId = "dtp-" + id;
        id++;
        setTimeout(() => {
            let dp = $('#' + this.myId).datetimepicker();
            dp.on('dp.change', this.onChange);
            this.updateDate();
        })
    },
    destroyed() {
        let dp = $('#' + this.myId);
        dp.off('dp.change', this.onChange);
    },
    methods: {
        onChange(ev) {
            let date = ev.date;
            let value = this.value;
            if(value === null || value === undefined){
                this.$emit("input", date);
                return;
            }
            date.seconds(0);
            value.seconds(0);
            if (date.format('YYYYMMDDHHmm') !== value.format('YYYYMMDDHHmm')) {
                this.$emit("input", date);
            }
        },
        updateDate() {
            let self = this;
            let runLater = () => {
                setTimeout(function () {
                    self.updateDate();
                }, 100);
            }
            let dp = $('#' + this.myId);
            if (dp === undefined) {
                runLater();
                return;
            }
            let data = dp.data("DateTimePicker");
            if (data === undefined) {
                runLater();
                return;
            }
            data.date(this.value);
        }
    },
    watch: {
        "value": 'updateDate',
    },
}
</script>

<style scoped>
.my-form-group {
  margin-bottom: 3px;
}
</style>