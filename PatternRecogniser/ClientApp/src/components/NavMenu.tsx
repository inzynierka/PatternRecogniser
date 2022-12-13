import './NavMenu.css';
import 'antd/dist/antd.min.css';

import { UserOutlined } from '@ant-design/icons';
import { Button, Dropdown, Layout, Menu, message, Row } from 'antd';
import { Link, useLocation } from 'react-router-dom';
import { NavLink } from 'reactstrap';
import { LogOut, LogOutReason } from '../pages/Account/LogOut';

const { Header } = Layout;

export function NavMenu() {
  const location = useLocation(); 
  let isUserAuthenticated = localStorage.getItem('token') !== null;

  const getSelectedKeyFromPath = () => {
    let path = location.pathname;
    if(path.includes('train')) return ['Train'];
    if(path.includes('recognise')) return ['Recognise'];
    if(path.includes('my-models')) return ['MyModels'];
    if(path.includes('comparison-lists')) return ['ComparisonLists'];
    return ['None'];
  }
  const navigateTo_IfLoggedIn = (to : string) => {
    return isUserAuthenticated ? to : "/login"
  }
  const logout = () => {
    LogOut(LogOutReason.userLoggedOut);
  }

  const accountMenu = (
    <Menu theme="dark" mode="vertical" data-testid="account-menu">
        <Menu.Item key="MyAccount"><NavLink tag={Link} to={"/my-account/" + localStorage.getItem('userId')}>Moje konto</NavLink></Menu.Item>
        <Menu.Item key="Logout" onClick={logout}><NavLink tag={Link} to="/login">Wyloguj</NavLink></Menu.Item>             
    </Menu>
  );

  return (
    <Header >
      { isUserAuthenticated &&  

        <Row className='mainMenu' justify="space-between" style={{width: "95vw"}} data-testid="main-menu">
            <Menu theme="dark" mode="horizontal" selectedKeys={getSelectedKeyFromPath()} style={{width: "60vw", height: '64px'}}>
                <Menu.Item key="Train" ><NavLink tag={Link} to={navigateTo_IfLoggedIn("/train")}>Trenuj model</NavLink></Menu.Item>
                <Menu.Item key="Recognise" ><NavLink tag={Link} to={navigateTo_IfLoggedIn("/recognise")}>Rozpoznaj znak</NavLink></Menu.Item>
                <Menu.Item key="MyModels" ><NavLink tag={Link} to={navigateTo_IfLoggedIn("/my-models")}>Moje modele</NavLink></Menu.Item>
                <Menu.Item key="ComparisonLists" ><NavLink tag={Link} to={navigateTo_IfLoggedIn("/comparison-lists")}>Porównywarka</NavLink></Menu.Item>
            </Menu>

            <div className="user-avatar">
              <Dropdown overlay={accountMenu} placement="bottomRight" trigger={['click']}>
                <Button icon={<UserOutlined />} shape="circle"/>
              </Dropdown>
            </div> 
        </Row>
      }
    </Header>
  );
}
