import { NavComponent, NavComponentProps, navHOC } from '@/components/hoc/NavComponent';
import Endpoints from '@/endpoints';
import Player from '@/types/dto/player';
import { empty } from '@/types/other';
import axios from 'axios';
import React from 'react';

interface ILoginPageState {
	username: string;
	password: string;
	nickname: string;
}

type LoginPayload = {
	username: string;
	password: string;
};

class LoginPage extends NavComponent<empty, ILoginPageState> {
	constructor(props: NavComponentProps<empty>) {
		super(props);
		this.state = {
			username: '',
			password: '',
			nickname: ''
		};
	}

	playClickedHandler = async () => {
		const data: Player = {
			guid: '00000000-0000-0000-0000-000000000000',
			nickname: this.state.nickname
		};

		await axios.post(Endpoints.Player, data);

		this.props.navigate('/rooms');
	};

	loginHandler = async () => {
		try {
			const data: LoginPayload = {
				username: this.state.username,
				password: this.state.password
			};

			await axios.post(Endpoints.Login, data);

			this.props.navigate('/rooms');
		} catch (err) {
			console.error(err);
		}
	};

	render(): React.ReactNode {
		return (
			<div className="container">
				<h1>Sign in ...</h1>
				<div className="form">
					<div className="form-group">
						<label>Login: </label>
						<input
							className="form-control"
							type="text"
							value={this.state.username}
							onInput={e => {
								this.setState({ username: e.currentTarget.value });
							}}
						/>
					</div>
					<div className="form-group">
						<label>Password: </label>
						<input
							className="form-control"
							type="password"
							value={this.state.password}
							onInput={e => {
								this.setState({ password: e.currentTarget.value });
							}}
						/>
					</div>
					<button
						className="btn btn-primary mt-3"
						onClick={() => {
							this.loginHandler();
						}}
					>
						Login
					</button>
					<div>
						<span>
							Don&apos;t have an account? <a href="/register">Sign up</a>
						</span>
					</div>
				</div>

				<h1>... or play anonymously</h1>
				<form>
					<div className="form-group">
						<label>Nick: </label>
						<input
							className="form-control"
							type="text"
							value={this.state.nickname}
							onInput={e => {
								this.setState({ nickname: e.currentTarget.value });
							}}
						/>
					</div>
				</form>
				<div>
					<button className="btn btn-primary mt-3" onClick={async () => await this.playClickedHandler()}>Play</button>
				</div>
			</div>
		);
	}
}

export default navHOC(LoginPage);
