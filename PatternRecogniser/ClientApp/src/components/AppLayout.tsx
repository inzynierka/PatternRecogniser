import { Component } from 'react';
import { NavMenu } from './NavMenu';
import 'antd/dist/antd.min.css';

import '.././custom.css';
import { Layout } from 'antd';
const { Content, Footer } = Layout;

export class AppLayout extends Component {
  static displayName = AppLayout.name;

  render () {
    return (
      <Layout className="layout" style={{ display: 'flex', flexDirection: 'column', flex: 1 }}>
        <NavMenu />
        <Content style={{ padding: '0 50px', flex: 1}}>
          {this.props.children}
        </Content>
        <Footer style={{ textAlign: 'center' }}>
          Ant Design ©2018 Created by Ant UED <br /> <br />
          Aplikacja opracowana w ramach pracy inżynierskiej.
        </Footer>
      </Layout>
    );
  }
}
