import React from 'react';

import { empty } from '@/types/other';
import axios from "axios";
import Endpoints, {Base} from "@/endpoints";
import Player from "@/types/dto/player";

interface ILoginPageState {
	username: string;
	password: string;
	nick: string;
}

class LoginPage extends React.Component<empty, ILoginPageState> {
	constructor(props: empty) {
		super(props);
		this.state = {
			username: '',
			password: '',
			nick: ''
		};
	}

	playClickedHandler = async () => {
		const data: Player = {
			guid: '00000000-0000-0000-0000-000000000000',
			nickname: this.state.nick
		};
		const resp = await axios.post(Endpoints.Player, data);
		console.log(resp);
	}
	
	render(): React.ReactNode {
		return (
			<div>
				<h1>Login ...</h1>
				<div>
					<label>Login: </label>
					<input type="text" value={this.state.username} />
				</div>
				<div>
					<label>Password: </label>
					<input type="password" value={this.state.password} />
				</div>
				<div>
					<a>Login</a>
				</div>
				<div>
					<span>
						Don't have an account? <a href="/register">Register</a>
					</span>
				</div>
				<h1>... or play anonymously</h1>
				<div>
					<label>Nick: </label>
					<input type="text" value={this.state.nick} onInput={(e) => {
						this.setState({nick: e.currentTarget.value});
					}} />
				</div>
				<div>
					<a onClick={async () => await this.playClickedHandler()}>Play</a>
				</div>
			</div>
		);
	}
}

export default LoginPage;
