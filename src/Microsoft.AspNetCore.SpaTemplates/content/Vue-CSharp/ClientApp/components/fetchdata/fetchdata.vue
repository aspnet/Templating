<template>
    <div>
        <h1>Weather forecast</h1>

        <p>This component demonstrates fetching data from the server.</p>

        <table v-if="forecasts.length" class="table">
            <thead>
                <tr>
                    <th>Date</th>
                    <th>Temp. (C)</th>
                    <th>Temp. (F)</th>
                    <th>Summary</th>
                </tr>
            </thead>
            <tbody>
                <tr v-for="(item, index) in forecasts" :key="index">
                    <td>{{ item.dateFormatted }}</td>
                    <td>{{ item.temperatureC }}</td>
                    <td>{{ item.temperatureF }}</td>
                    <td>{{ item.summary }}</td>
                </tr>
            </tbody>
        </table>

        <p v-else><em>Loading...</em></p>
    </div>
</template>

<script lang="ts" src="./fetchdata.ts"></script>
