<template>
    <div class="row">
        <div class="row">
            <div class='col-md-1'>
                <button class="btn btn-info update-button" v-on:click="updateClicked">Update</button>
            </div>
            <div class='col-md-7 col-lg-6 col-sm-8'>
                <div class='col-md-5'>
                    <label>
                        Start Date:
                    </label>
                    <date-time-picker v-model='startTime'></date-time-picker>
                </div>
                <div class='col-md-5'>
                    <label>
                        End Date:
                    </label>
                    <date-time-picker v-model='endTime'></date-time-picker>
                </div>
            </div>
        </div>
        <div class="row button-row">
                <button class="btn btn-info slim-button" v-on:click="addTime('days')">+1 Day</button>
                <button class="btn btn-info slim-button" v-on:click="addTime('weeks')">+1 Week</button>
                <button class="btn btn-info slim-button" v-on:click="addTime('months')">+1 Month</button>
                <button class="btn btn-info slim-button" v-on:click="addTime('years')">+1 Year</button>
                <button class="btn btn-info slim-button" style="margin-left:30px;" v-on:click="thisDay">Today</button>
                <button class="btn btn-info slim-button" v-on:click="thisWeek">This Week</button>
                <button class="btn btn-info slim-button" v-on:click="thisMonth">This Month</button>
                <button class="btn btn-info slim-button" v-on:click="thisYear">This Year</button>
        </div>
        <div class="row button-row">
                <button class="btn btn-info slim-button" v-on:click="subTime('days')">-1 Day</button>
                <button class="btn btn-info slim-button" v-on:click="subTime('weeks')">-1 Week</button>
                <button class="btn btn-info slim-button" v-on:click="subTime('months')">-1 Month</button>
                <button class="btn btn-info slim-button" v-on:click="subTime('years')">-1 Year</button>
        </div>
    </div>
</template>

<script>
import api from "../js/api.js";
import router from "../js/router.js";
import moment from "moment";
import DateTimePicker from "./date-time-picker.vue";

export default {
    props: ['defaultRangeDays', 'useQuery'],
    data() {
        return {
            offsetHours: 0,
            startTime: new moment(),
            endTime: new moment(),
        };
    },
    created() {
        this.offsetHours = 0;
        let end = moment().startOf("day").add(this.offsetHours, "hours").add(1, "day");
        let start = moment().startOf("day").add(this.offsetHours, "hours").add(1, "day");
        let defRange = this.defaultRangeDays;
        if (defRange === null || defRange === undefined) {
            defRange = 1;
        }
        defRange = -1 * Math.abs(defRange);
        start.add(defRange, "days");
        this.startTime = start;
        this.endTime = end;

        if(this.useQuery){
            try{
                let d1 = this.$route.query.startTime;                        
                let d2 = this.$route.query.endTime;                        
                if(d1 && d2){
                    let m1 = new moment(d1);
                    let m2 = new moment(d2);
                    this.startTime = m1;
                    this.endTime = m2;
                }
            }
            catch(ex){
            }
        }

        this.$emit("done-loading", this.startTime, this.endTime);
    },
    methods: {
        updateClicked() {
            this.$emit("update-clicked", this.startTime, this.endTime);
        },
        setAndUpdate(start, end) {
            this.startTime = start;
            this.endTime = end;
            if(this.useQuery){
                try{
                    router.appendQuery(this, { startTime: this.startTime.toISOString(), endTime: this.endTime.toISOString() })
                }
                catch(ex){}
            }
            this.$emit("update-clicked", this.startTime, this.endTime);
        },
        addTime(momentSpecifier) {
            this.setAndUpdate(new moment(this.startTime).add(1, momentSpecifier), new moment(this.endTime).add(1, momentSpecifier));
        },
        subTime(momentSpecifier) {
            this.setAndUpdate(new moment(this.startTime).add(-1, momentSpecifier), new moment(this.endTime).add(-1, momentSpecifier));
        },
        thisX(momentSpecifier) {
            let start = moment().startOf(momentSpecifier).add(this.offsetHours, "hours");
            let end = moment().endOf(momentSpecifier).startOf("day").add(this.offsetHours, "hours").add(1, "day");
            this.setAndUpdate(start, end);
        },
        thisDay() {
            this.thisX("day");
        },
        thisWeek() {
            this.thisX("week");
        },
        thisMonth() {
            this.thisX("month");
        },
        thisYear() {
            this.thisX("year");
        },
    },
    components: {
        DateTimePicker
    }
};
</script>

<style scoped>
.update-button {
  margin-top: 25px;
}
.slim-button {
  padding-top: 2px;
  padding-bottom: 2px;
  font-family: monospace;
}
.button-row {
  margin-left: 3%;
  margin-bottom: 1px;
}
</style>