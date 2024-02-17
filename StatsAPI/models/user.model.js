const mongoose = require('mongoose');

const userSchema = new mongoose.Schema({
  Id: String,
  Username: String,
  Password: String
});

const User = mongoose.model('User', userSchema);
module.exports = User;