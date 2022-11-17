import { useContext, useState } from 'react';
import { NavLink } from 'reactstrap';
import { useLocation, Link } from 'react-router-dom';
import './NavMenu.css';
import 'antd/dist/antd.min.css';
import { UserOutlined } from '@ant-design/icons';

import { Button, Layout, Menu, message, Row } from 'antd';
import { globalContext } from '../reducers/GlobalStore';
const { Header } = Layout;

export function NavMenu() {
  const [avatarClicked, setAvatarClicked] = useState(false);
  const location = useLocation(); 
  const { globalState, dispatch } = useContext(globalContext);

  const getSelectedKeyFromPath = () => {
    let path = location.pathname;
    if(path.includes('train')) return ['Train'];
    if(path.includes('recognise')) return ['Recognise'];
    if(path.includes('my-models')) return ['MyModels'];
    if(path.includes('comparison-lists')) return ['ComparisonLists'];
    return ['None'];
  }
  const navigateTo_IfLoggedIn = (to : string) => {
    return globalState.isUserAuthenticated ? to : "/login"
  }
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

        <Row justify="space-between" style={{width: "95vw"}}>
              <Menu theme="dark" mode="horizontal" selectedKeys={getSelectedKeyFromPath()} style={{width: "50vw", height: '64px'}}>
                  <Menu.Item key="Train" ><NavLink tag={Link} to={navigateTo_IfLoggedIn("/train")}>Trenuj model</NavLink></Menu.Item>
                  <Menu.Item key="Recognise" ><NavLink tag={Link} to={navigateTo_IfLoggedIn("/recognise")}>Rozpoznaj znak</NavLink></Menu.Item>
                  <Menu.Item key="MyModels" ><NavLink tag={Link} to={navigateTo_IfLoggedIn("/my-models")}>Moje modele</NavLink></Menu.Item>
                  <Menu.Item key="ComparisonLists" ><NavLink tag={Link} to={navigateTo_IfLoggedIn("/comparison-lists")}>Moje modele</NavLink></Menu.Item>
              </Menu>

              <div className="user-avatar">
                  <Button icon={<UserOutlined />} shape="circle" onClick={avatarClickHandle}/>
                {
                  globalState.isUserAuthenticated && avatarClicked &&
                  <Menu theme="dark" mode="vertical">
                      <Menu.Item key="logout" onClick={logout}><NavLink tag={Link} to="/login">Wyloguj</NavLink></Menu.Item>            
                  </Menu>
                }
                
              </div> 
          </Row>
        }
    </Header>
  );
}
