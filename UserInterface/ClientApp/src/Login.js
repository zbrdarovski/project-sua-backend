import React, { useState } from 'react';
import './Form.css';
import { useNavigate } from 'react-router-dom';

function Login() {
    const [formData, setFormData] = useState({ username: '', password: '' });

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({ ...formData, [name]: value });
    };

    const navigate = useNavigate();

    const handleLogin = async (e) => {
        e.preventDefault();
        try {
            const response = await fetch('http://localhost:5293/api/users/login', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                },
                body: JSON.stringify({
                    username: formData.username,
                    password: formData.password,
                }),
            });

            if (!response.ok) {
                // Handle error cases
                console.error('Login failed:', response.statusText);
                return;
            } else navigate('/shop');
        } catch (error) {
            console.error('Login failed:', error.message);
        }
    };

    const handleClick = () => {
        navigate('/register');
    };

    return (
        <div className='container'>
            <h2>Login</h2>
            <form onSubmit={handleInputChange}>
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
                    name="password"
                    placeholder="Password"
                    value={formData.password}
                    onChange={handleInputChange}
                    autoComplete={"off"}
                />
                <button onClick={handleLogin}>Login</button>
            </form>
            <p onClick={handleClick} style={{ cursor: 'pointer' }}>
                Register
            </p>
        </div>
    );
}

export default Login;