<template>
    <span>
        <template v-if="start">
            <span class="date" v-if="isToday">Today</span>
            <span class="date" v-text="startDate" v-if="!isToday"></span>
            <span class="time" v-if="!noTime" v-text="startTime"></span>
        </template>
        <span class="to" v-if="start && end"> to </span>
        <template v-if="end">
            <span class="date" v-text="endDate" v-if="!isSameDay"></span>
            <span class="time" v-if="!noTime" v-text="endTime"></span> 
        </template>
    </span>
</template>

<script>
import moment from "moment";

export default {
    props: ['start', 'end', 'noTime'],
    data() {
        return {
        };
    },
    computed: {
        startM(){
            return new moment(this.start);
        },
        endM(){
            return new moment(this.end);
        },
        isToday(){
            return this.startM.isSame(new moment(), "day");
        },
        isSameDay(){
            return this.startM.isSame(this.endM, "day");
        },
        startTime(){
            return this.startM.format('h:mm A');
        },
        endTime(){
            return this.endM.format('h:mm A');
        },
        startDate(){
            return this.startM.format('M/D');
        },
        endDate(){
            return this.endM.format('M/D');
        },
    },
    methods: {
    },
    components: {}
};
</script>

<style scoped>
    .to{
        font-style: italic;
    }
    .date{
        font-weight: bold;
    }
</style>