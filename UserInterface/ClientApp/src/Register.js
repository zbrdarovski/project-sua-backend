import React, { useState, useEffect } from 'react';
import './Form.css';
import { useNavigate } from 'react-router-dom';

const Register = () => {
    const [formData, setFormData] = useState({
        username: '',
        password: '',
        repeatPassword: ''
    });

    const [highestUserId, setHighestUserId] = useState(0);

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const navigate = useNavigate();

    useEffect(() => {
        const fetchAllUsers = async () => {
            try {
                const response = await fetch('http://localhost:5293/api/users/get-all');
                if (response.ok) {
                    const data = await response.json();
                    if (data.length > 0) {
                        const maxId = Math.max(...data.map(user => user.id));
                        setHighestUserId(maxId);
                    } else {
                        // If there are no users, set the highestUserId to 0 or 1, depending on your server's expectations
                        setHighestUserId(0);
                    }
                } else {
                    console.error('Failed to fetch all users:', response.statusText);
                }
            } catch (error) {
                console.error('Failed to fetch all users:', error.message);
            }
        };

        fetchAllUsers();
    }, []); // Fetch all users once when the component mounts

    const handleRegister = async (e) => {
        e.preventDefault();

        try {
            if (
                formData.username.length >= 6 &&
                formData.password.length >= 6 &&
                formData.password === formData.repeatPassword
            ) {
                const response = await fetch('http://localhost:5293/api/users/register', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                    },
                    body: JSON.stringify({
                        id: (highestUserId + 1).toString(),
                        username: formData.username,
                        password: formData.password
                    }),
                });

                if (!response.ok) {
                    console.error('Registration failed:', response.statusText);
                    return;
                } else {
                    console.log('Registration successful');
                    navigate('/');
                }
            } else {
                console.warn('Form validation failed');
            }
        } catch (error) {
            console.error('Registration failed:', error.message);
        }
    };

    const handleClick = () => {
        navigate('/');
    };

    return (
        <div className='container'>
            <h2>Register</h2>
            <form onSubmit={handleRegister}>
                <input
                    type="text"
                    placeholder="Enter username"
                    name="username"
                    value={formData.username}
                    onChange={handleInputChange}
                />
                <input
                    type="password"
                    placeholder="Enter password"
                    name="password"
                    value={formData.password}
                    onChange={handleInputChange}
                />
                <input
                    type="password"
                    placeholder="Repeat password"
                    name="repeatPassword"
                    value={formData.repeatPassword}
                    onChange={handleInputChange}
                />
                <button type="submit">Register</button>
            </form>
            <p onClick={handleClick} style={{ cursor: 'pointer' }}>
                Login
            </p>
        </div>
    );
};

export default Register;