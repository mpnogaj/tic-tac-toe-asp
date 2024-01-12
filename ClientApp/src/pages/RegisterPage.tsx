import { NavComponent, NavComponentProps, navHOC } from '@/components/hoc/NavComponent';
import Endpoints from '@/endpoints';
import { empty } from '@/types/other';
import axios from 'axios';
import React from 'react';

interface IRegisterPageState {
	username: string;
	password: string;
	nickname: string;
}

type RegisterPayload = {
	username: string;
	password: string;
	nickname: string;
};

class RegisterPage extends NavComponent<empty, IRegisterPageState> {
	constructor(props: NavComponentProps<empty>) {
		super(props);

		this.state = {
			username: '',
			password: '',
			nickname: ''
		};
	}

	registerHandler = async () => {
		try {
			const data: RegisterPayload = {
				username: this.state.username,
				nickname: this.state.nickname,
				password: this.state.password
			};

			await axios.post(Endpoints.Register, data);
			this.props.navigate('/login');
		} catch (err) {
			console.error(err);
		}
	};

	render(): React.ReactNode {
		return (
			<div>
				<h1>Register</h1>
				<div>
					<label>Username: </label>
					<input
						type="text"
						value={this.state.username}
						onChange={event => {
							this.setState({ username: event.target.value });
						}}
					/>
				</div>
				<div>
					<label>Nickname: </label>
					<input
						type="text"
						value={this.state.nickname}
						onChange={event => {
							this.setState({ nickname: event.target.value });
						}}
					/>
				</div>
				<div>
					<label>Password: </label>
					<input
						type="password"
						value={this.state.password}
						onChange={event => {
							this.setState({ password: event.target.value });
						}}
					/>
				</div>
				<div>
					<a onClick={async () => await this.registerHandler()}>Register</a>
				</div>
			</div>
		);
	}
}

export default navHOC(RegisterPage);
