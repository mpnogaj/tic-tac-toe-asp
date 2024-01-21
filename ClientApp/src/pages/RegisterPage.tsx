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
			<div className="container">
				<h1>Register</h1>
				<form>
					<div className="form-group">
						<label>Username: </label>
						<input
							className="form-control"
							type="text"
							value={this.state.username}
							onChange={event => {
								this.setState({ username: event.target.value });
							}}
						/>
					</div>
					<div className="form-group">
						<label>Nickname: </label>
						<input
							className="form-control"
							type="text"
							value={this.state.nickname}
							onChange={event => {
								this.setState({ nickname: event.target.value });
							}}
						/>
					</div>
					<div className="form-group">
						<label>Password: </label>
						<input
							className="form-control"
							type="password"
							value={this.state.password}
							onChange={event => {
								this.setState({ password: event.target.value });
							}}
						/>
					</div>
					<button className="btn btn-primary mt-3" onClick={async () => await this.registerHandler()}>Register</button>
				</form>
			</div>
		);
	}
}

export default navHOC(RegisterPage);
