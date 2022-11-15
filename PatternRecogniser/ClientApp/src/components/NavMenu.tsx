import { useContext, useState } from 'react';
import { NavLink } from 'reactstrap';
import { useLocation, Link } from 'react-router-dom';
import './NavMenu.css';
import 'antd/dist/antd.min.css';
import { UserOutlined } from '@ant-design/icons';

import { Button, Layout, Menu, message } from 'antd';
import { globalContext } from '../reducers/GlobalStore';
const { Header } = Layout;

export function NavMenu() {
  const [avatarClicked, setAvatarClicked] = useState(false);
  const location = useLocation(); 
  const { globalState, dispatch } = useContext(globalContext);

  const getSelectedKeyFromPath = () => {
    let path = location.pathname;
    if(path.includes('read')) return ['ReadBooks'];
    if(path.includes('recommended')) return ['RecommendedBooks'];
    if(path.includes('all')) return ['Books'];
    return ['None'];
  }
  // const navigateTo_IfLoggedIn = (to : string) => {
  //   return globalState.isUserAuthenticated ? to : "/login"
  // }
  const avatarClickHandle = () => {
    setAvatarClicked(!avatarClicked);
  }
  const logout = () => {
    dispatch({ type: 'AUTHENTICATE_USER', payload: false });
    setAvatarClicked(false);
    message.success('Logged out succesfully!');
  }

  return (
    <Header >
       { globalState.isUserAuthenticated &&  
        <div className="user-avatar">
          <Button icon={<UserOutlined />} shape="circle" onClick={avatarClickHandle}/>
        </div> }
     
      <Menu theme="dark" mode="horizontal" selectedKeys={getSelectedKeyFromPath()}>
          {
            globalState.isUserAuthenticated && avatarClicked &&
              <Menu.Item key="logout" onClick={logout}><NavLink tag={Link} to="/login">Wyloguj</NavLink></Menu.Item>
          }
      </Menu>
      
    </Header>
  );
}
