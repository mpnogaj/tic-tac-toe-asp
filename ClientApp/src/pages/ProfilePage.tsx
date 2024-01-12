import LoadingComponent from '@/components/LoadingComponent';
import Endpoints from '@/endpoints';
import Account from '@/types/dto/account';
import { empty } from '@/types/other';
import axios from 'axios';
import React from 'react';

type CompState = {
	error: string | undefined;
	accountInfo: Account | undefined;
	nickname: string;
	newPassword: string;
	newPasswordConfirm: string;
	oldPassword: string;
};

class ProfilePage extends React.Component<empty, CompState> {
	constructor(props: empty) {
		super(props);

		this.state = {
			error: undefined,
			accountInfo: undefined,
			nickname: '',
			newPassword: '',
			newPasswordConfirm: '',
			oldPassword: ''
		};
	}

	async componentDidMount(): Promise<void> {
		try {
			const accountInfo = (await axios.get<Account>(Endpoints.Auth)).data;
			const nickname = (await axios.get<string>(Endpoints.Player)).data;

			this.setState({
				...this.state,
				accountInfo: accountInfo,
				nickname: nickname
			});
		} catch (err) {
			let errorMsg = 'Unknown error';
			if (err instanceof Error) {
				errorMsg = err.message;
			}
			console.error(errorMsg);
			this.setState({ ...this.state, error: errorMsg });
		}
	}

	passwordsMatches = () => {
		return this.state.newPassword === this.state.newPasswordConfirm;
	};

	render() {
		if (this.state.error !== undefined) {
			return (
				<div>
					<h1>Error</h1>
					{this.state.error}
				</div>
			);
		}

		if (this.state.accountInfo === undefined) {
			return <LoadingComponent />;
		}

		return (
			<div>
				<h1>Your account - {this.state.accountInfo.username}</h1>
				<div>
					<h2>Edit data</h2>
					<div>
						<div>
							<label>Nickname</label>
							<input
								type="text"
								value={this.state.nickname}
								onChange={e => {
									this.setState({ ...this.state, nickname: e.target.value });
								}}
							/>
						</div>

						<div>
							<label>New password (leave blank if you don&apos;t want to edit)</label>
							<input
								type="password"
								value={this.state.newPassword}
								onChange={e => {
									this.setState({ ...this.state, newPassword: e.target.value });
								}}
							/>
						</div>
						<div>
							<label>Confirm new password</label>
							<input
								type="password"
								value={this.state.newPasswordConfirm}
								onChange={e => {
									this.setState({ ...this.state, newPasswordConfirm: e.target.value });
								}}
							/>
						</div>
						<span
							style={{
								color: 'red',
								display: this.passwordsMatches() ? 'none' : 'block'
							}}
						>
							Password doesn&apos;t match
						</span>

						<div>
							<label>Current password</label>
							<input
								type="password"
								value={this.state.oldPassword}
								onChange={e => {
									this.setState({ ...this.state, oldPassword: e.target.value });
								}}
							/>
						</div>
						<a href="#">Submit</a>
					</div>
				</div>
				<div>
					<h2>Stats</h2>
					<div>
						<span>Matches played: {this.state.accountInfo.matchesPlayed}</span>
					</div>
					<div>
						<span>Matches won: {this.state.accountInfo.matchesWon}</span>
					</div>
				</div>
			</div>
		);
	}
}

export default ProfilePage;
