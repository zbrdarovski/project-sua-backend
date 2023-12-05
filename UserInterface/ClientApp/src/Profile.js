import Dashboard from './Dashboard';
import React, { useState } from 'react';
import './Form.css';
import { useNavigate } from 'react-router-dom';

function Profile() {
    const [formData, setFormData] = useState({
        username: '', currentPassword: '', newPassword: '', repeatNewPassword: ''
    });

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const navigate = useNavigate();

    const handleChangePassword = async () => {
        try {
            const response = await fetch('http://localhost:5293/api/users/change-password', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    username: formData.username,
                    currentPassword: formData.currentPassword,
                    newPassword: formData.newPassword
                }),
            });

            if (!response.ok) {
                // Handle error cases
                console.error('Login failed:', response.statusText);
                return;
            } else {
                navigate('/shop');
            }
        } catch (error) {
            console.error('Login failed:', error.message);
        }
    };

    return (
        <div className="Profile">
            <Dashboard />
            <div className="container">
                <h2>Password</h2>
                <input
                    type="text"
                    name="username"
                    placeholder="Username"
                    value={formData.username}
                    onChange={handleInputChange}
                    autoComplete={"on"}
                />
                <input
                    type="password"
                    name="currentPassword"
                    placeholder="Current password"
                    value={formData.currentPassword}
                    onChange={handleInputChange}
                    autoComplete={"off"}
                />
                <input
                    type="password"
                    name="newPassword"
                    placeholder="New password"
                    value={formData.newPassword}
                    onChange={handleInputChange}
                    autoComplete={"off"}
                />
                <input
                    type="password"
                    name="repeatNewPassword"
                    placeholder="Repeat new password"
                    value={formData.repeatNewPassword}
                    onChange={handleInputChange}
                    autoComplete={"off"}
                />
                <button
                    disabled={
                        !(
                            formData.username.length > 0 &&
                            formData.currentPassword.length > 0 &&
                            formData.newPassword.length >= 6 &&
                            formData.newPassword === formData.repeatNewPassword
                        )
                    }
                    onClick={handleChangePassword}
                >
                    Change
                </button>
            </div>
        </div>
    );
}

export default Profile;