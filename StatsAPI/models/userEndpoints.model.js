const mongoose = require('mongoose');

const statsEndpointsSchema = new mongoose.Schema({
    timestamp: Date,
    endpoint: String,
});

const StatsEndpoints = mongoose.model('UserEndpoints', statsEndpointsSchema);
module.exports = StatsEndpoints;