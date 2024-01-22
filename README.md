# The Zero.js Spec

<br />
<br />

${\Huge{\textsf{Build\ for\ the\ web\ using\ {\color{Orange}zero}}\ \textsf{JavaScript.}}}$

<br />
<br />

Basically, Zero.js is just vanilla HTML with a few small additions. Its purpose is to reduce the web's dependency on JavaScript by opening the door for other languages to compete. But making JavaScript interchangeable is unrealistic unless other languages can both _generate HTML_ **AND** _manipulate the DOM_ using one common approach. The key is using an HTML-first strategy. Instead of putting HTML inside your logic, Zero.js puts your logic inside your HTML.

In many ways, Zero.js is like Markdown. It's not an implementation but rather a small set of rules for outputting predictable results for the web regardless of language choice. Like Markdown, there's also room for various "flavors" to bring their own special embellishments.

Zero.js's purpose is grounded in the desire for the web to remain **THE** melting pot of human ideas and progress. The best way to prevent stagnation is to open the floodgates for other languages to compete.

> [!IMPORTANT]
> The examples in this README use a fictional language called `AnyScript` in order to provide concrete examples without favoring any particular language. Conceptually, any imperative language could be substituted.

### Table of Contents

- [Components](#components)
- [Children](#children)
- [Control Flow](#control-flow)
- [Styles](#styles)
- [Routing](#routing)
- [Bring Your Own Language (BYOL)](#byol-bring-your-own-language)
- [State Management](#state-management)
- [Ecosystem](#ecosystem)

<br />
<br />

## Components

### File-Based

Every `.html` file is automatically a component. No need to register or import anything. Just use its file name. (Markdown files make for great components too.)

<table>
<tr>
<td>
<code>my-button.html</code>                                
</td>
<td>
<code>index.html</code>                                                
</td>
</tr>
<tr>
<td>

```xml
<button>
  Click me
</button>
```

</td>
<td>

```xml
<html>
  <body>
    <my-button />
  </body>
</html>
```

</td>
</tr>
</table>

> [!NOTE]
> File names are limited to alpha-numerics, hyphens and underscores.
> Read more about resolving [naming collisions](https://todo) below.

### Attributes

HTML attributes are an easy way to pass inputs into a component thus boosting reusability.

You'll notice the use of the `{{ }}` escape sequence in the component file. You'll learn more about this in the [Hole Punch](#hole-punch) section below.

<table>
<tr>
<td>
<code>index.html</code>
</td>
<td>
<code>my-button.html</code>                                
</td>
</tr>
<tr>
<td>

```xml
<html>
  <body>
    <my-button name="Rylan" />
  </body>
</html>
```

</td>
<td>

```xml
<button>
  Hello {{name}}
</button>
```

</td>
</tr>
</table>

> [!NOTE]
> A component's attributes are optional by default. The ability to denote some attributes as "required" is outside the scope of this document but is a highly recommended "flavor" to be provided by the guest language.

### Strongly Typed

In your attributes, when using data types other than strings, use JSON-like syntax where the double-quotes are excluded. Non-primitives, such as objects and arrays, must be handled inside a hole-punch `{{ }}` so that the specific syntax can vary by language.

```xml
index.html

<html>
  <body>
    <my-button text="Hello" count=1 weight=3.14 cta=true time={{date}} />
  </body>
</html>
```

### Sibling Files

Any file in the same directory with the same filename but different extension must be conceptually treated as a part of the same component. Rules for handling may vary according to file type.

For example, including a `.css` file makes it trivial to include styles with your component. However, should that component ever repeat its HTML, its CSS would only ever be included once.

Sibling files can also make use of hole punches `{{ }}`.

<table>
<tr>
<td>
<code>File Structure</code>
</td>
<td>
<code>index.html</code>                                
</td>
<td>
<code>Output</code>                                                                
</td>
</tr>
<tr>
<td>

- website
  - index.html
  - component.html
  - component.css

</td>
<td>

```xml
<html>
  <body>
    <component />
    <component />
    <component />
  </body>
</html>
```

</td>
<td>

```xml
<html>
  <body>
    <style>
      ...CSS content...
    </style>
    <div>
      ...HTML content...
    </div>
    <div>
      ...HTML content (repeated)...
    </div>
    <div>
      ...HTML content (repeated)...
    </div>
  </body>
</html>
```

</td>
</tr>
</table>

> [!NOTE]
> While sibling files can technically work with `.js` files too, that's not the recommended approach. This might be useful if you are certain your website will only ever be statically generated. But it's important to note that Zero.js has a different language-agnostic approach for building dynamic features that offers a gradual migration path from a fully static website to a fully dynamic web app. You'll learn more about this in the [BYOL](#🤖-byol-bring-your-own-language) section below.

<br />
<br />

## Children

### Content

To simplify composability, any child-tags to a component can be accessed using `{{content}}`. This is intentionally similar to attribute-usage. This makes `content` a reserved keyword and therefore cannot be used as a component's attribute.

<table>
<tr>
<td>
<code>index.html</code>
</td>
<td>
<code>my-button.html</code>                                
</td>
</tr>
<tr>
<td>

```xml
<html>
  <body>
    <my-button>
      <b>Please</b> click me
    </my-button>
  </body>
</html>
```

</td>
<td>

```xml
<button>
  {{content}}
</button>
```

</td>
</tr>
</table>

### Slots

It is possible to provide outside HTML as inputs to a component. However this isn't done through its `attribute`s but using `slot`s instead. Similar to Web Components, child-tags can be used as HTML-based inputs using the `slot` keyword.

Any tags without a `slot` attribute must default to the reserved slot `content`. Slot order does not matter and any missing slots are treated as `null`.

<table>
<tr>
<td>
<code>index.html</code>
</td>
<td>
<code>my-button.html</code>                                
</td>
</tr>
<tr>
<td>

```xml
<html>
  <body>
    <my-button>
      <img slot="icon" src="..." />
      <span slot="text">
        I am <i>more</i> than a <b>string</b>.
      </span>
    </my-button>
  </body>
</html>
```

</td>
<td>

```xml
<button>
  {{icon}} • {{text}}
</button>
```

</td>
</tr>
</table>

> [!NOTE]
> Slots are optional and if not supplied, the component must still function. Similar to attributes, though, it possible and recommended for a guest language to support "required inputs" much like a class with a non-nullable property.

<br />
<br />

## Control Flow

Zero.js uses a few tags for control flow: `<if>`, `<else>`, `<else-if>` and `<foreach>`. These are reserved keywords so custom components by those names are not allowed. While the content inside these tags might be included, excluded or repeated, the enclosing tag itself is never included in the generated HTML. (The browser wouldn't know what to do with an `<if>` tag anyway right?)

Most other frameworks choose to use their natural syntax instead of extending HTML to handle control flow. For them, this is a sensible choice since it allows for greater flexibility. Since the purpose of Zero.js is to be language-agnostic, it takes a more generic approach by simply extending HTML and to lean into its [hole-punching](#hole-punch) approach for extending functionality.

### Conditions

The `<if>` tag conditionally excludes its content. It uses a valueless attribute as the condition for terseness.

```xml
<if {{user == null}}>
  <sign-in-form />
</if>
```

Use `<else>` and `<else-if>` tags in conjunction with `<if>`. These are sibling tags and do not require a parent container as you see with `<ul>` and `<li>`.

```xml
<if {{loginState == .LOGGED_OUT}}>
  <sign-in-form />
</if>
<else-if {{loginState == .LOGGED_IN}}>
  <sign-out-button />
</else-if>
<else>
  <circular-progress />
</else>
```

### Pattern Matching

The `if-let` syntax lets you handle your dynamic content in a less verbose way. If the expression evaluates to `null`, all child content is excluded from the generated HTML. Otherwise, the compiler can safely assume non-nullability for the provided attribute name.

In this example, if any of `user`, `profile`, `name`, or `first` are null then the entire child contents are skipped, including the "Welcome back " text.

```xml
<if firstName={{user?.profile?.name?.first}}>
  Welcome back {{firstName}}.
</if>
```

Multiple null-checks are allowed as well as chaining with `else` and `else-if`.

```xml
<if first={{user?.firstName}} last={{user?.lastName}}>
  Welcome back {{first}} {{last}}.
</if>
<else>
  <h2>Loading...</h2>
</else>
```

### Loops

Use a `<foreach>` tag to repeat its contents in a declarative way.

```xml
<ul>
  <foreach fruit in={{allFruits}}>
    <li>{{fruit}}</li>
  </foreach>
</ul>
```

<br />
<br />

## Styles

### Platform and File-Based

CSS can be used normally, including embedded `<style>` tags and externally referenced `.css` files.

However, any `.css` file that shares the same filename and parent directory as an `.html` file is considered a [sibling file](#sibling-files-1) and must automatically have its styles included before any HTML to prevent any dreaded [FOUC](https://en.wikipedia.org/wiki/Flash_of_unstyled_content). Inclusion must occur only once per HTML document, since its `.html` counterpart might be repeated multiple times.

### Scoped Styles

Component-level styles must be scoped to its component. It is not required to use a Shadow DOM like Web Components do.

<table>
<tr>
<td>
<code>my-button.css</code>
</td>
<td>
<code>Output</code>                                
</td>
</tr>
<tr>
<td>

```css
p {
  font-size: 18pt;
}
em {
  color: orange;
}
```

</td>
<td>

```html
<html>
  <body>
    <style>
      .my-button p {
        font-size: 18pt;
      }
      .my-button em {
        color: orange;
      }
    </style>
    <button>
      <p>This button has <em>style</em>!</p>
    </button>
  </body>
</html>
```

</td>
</tr>
</table>

<br />
<br />

## Routing

### File-Based

Zero.js uses file-based routing as a language-agnostic way to define your URL routing patterns. Zero.js does encourage any "flavor" to embellish additional features in their own native language syntax.

### Directories

### Files

### Query String

### `POST`ing

<br />
<br />

## BYOL (Bring-Your-Own-Language)

### Migration Path from Static to Dynamic

Astute readers might notice that, at this point, we now have all the tools necessary to host or generate a static website composed of reusable components using only `.html` and `.css` files and nothing else.

This is a good thing. It ties the durability of your frontend to the longevity of the web itself. The more you can "embrace the platform" the less susceptible your codebase is to [software rot](https://en.wikipedia.org/wiki/Software_rot). It shouldn't be an unreasonable expectation to return to a 10 year old project and have everything function exactly as before.

The sections that follow cover a common approach for progressively enhancing your website using your language of choice.

### Event Handlers

Use the DOM's regular events for event handling but instead of specifying JavaScript inside a string, reference your own method using a hole-punch escape sequence `{{ }}`. This method can optionally choose to take the event as an argument.

```xml
<button onclick={{handleClick}}>
  Click me
</button>
```

Using closures can be a valid option too, if your language supports it.

```xml
<button onclick={{e => count++}}>
  Clicks: {{count}}
</button>
```

### `<script>` Tags

In the spirit of "embracing the platform" you can repurpose the `<script>` tag for use by any language using the ([to-spec](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/script#language)) `language` attribute.

When using the `<script>` tag for any language besides JavaScript, be sure to never include it as a part of any generated HTML. (The browser wouldn't know how to execute it anyway right?) Execution can be handled server-side or client-side via WebAssembly. More on that in the [Server-Side or Client-Side](#server-side-or-client-side) section.

```xml
my-button.html

<button onClick={{handleClick}}>
  Clicks: {{count}}
</button>

<script language="AnyScript">
  count = 0

  void handleClick(event) {
    count++
  }
</script>
```

> [!TIP]
> Whatever value is specified in the `language` attribute is technically moot since it's never included in the HTML. Therefore, whatever value is specified is more for human consumption than for the machine.

### Sibling Files

The sibling file approach works great for other languages. Any file in the same directory with the same filename but different file extension shall be treated as part of the same component. Rules regarding variable scoping and code-importing are intentionally undefined so that they can vary by language.

<table>
<tr>
<td>
<code>my-button.html</code>                                                                
</td>
<td>
<code>my-button.any</code>                                                                
</td>
</tr>
<tr>
<td>

```xml
<button onClick={{handleClick}}>
  Clicks: {{count}}
</button>
```

</td>
<td>

```c
count = 0

void handleClick(event) {
  count++
}
```

</td>
</tr>
</table>

### Hole Punch

The "hole punch" pattern `{{ }}` is familiar since it appears like a regular [string interpolation](https://en.wikipedia.org/wiki/String_interpolation). However, in order to excel at both HTML-generation and DOM-manipulation there is one important distinction. Instead of simply returning a final string, it returns a "composition object" which simply just hangs onto the inputs for later use. Once a composition is built, it becomes trivial to either lazily generate the HTML in full or to compare its input values with an older composition's input values for anything that might have changed so that it may generate instructions needed for updating the DOM.

The advantages to this approach are outside the scope of this spec but can be explored in-depth in [this article](https://rylan.io/blog/zero-virtual-dom) (coming soon). To summarize:

1. **Simplicity** - state changes don't require scope tracking
1. **Derived data** - compositions compare the inline expression values, not the state itself
1. **Portability** - works equally well in WASM as server-side since it does not depend on a DOM tree to re-compose
1. **Event Handling** - DOM events are naturally fit for [marshalling](<https://en.wikipedia.org/wiki/Marshalling_(computer_science)>)
1. **Precision** - can modify the values of individual DOM nodes instead of only brute-forcing large HTML fragments at a time
1. **Speed** - no virtual DOM necessary
1. **Memory** - no server-side node tree construction necessary

### Server-Side or Client-Side?

Why not both?

Zero.js follows a [Unidirectional data flow](https://developer.android.com/jetpack/compose/architecture#udf) design pattern.

> A unidirectional data flow (UDF) is a design pattern where state flows down and events flow up. By following unidirectional data flow, you can decouple composables that display state in the UI from the parts of your app that store and change state.

Thankfully the DOM's event-handling model encapsulates events as objects. This makes them a natural fit for [marshalling](<https://en.wikipedia.org/wiki/Marshalling_(computer_science)>)
back and forth to your language of choice running remotely on the server or running locally as WebAssembly. Additionally, if your transport supports bi-directional communication (e.g. WebSockets), the server can naturally react to state changes initiated from sources other than the browser (an inherent limitation of HTTP's request-response model).

<br />
<br />

## State Management

Zero.js is intentionally non-prescriptive when it comes to state management beyond simply using hole-punch escape sequences `{{ }}` as the contact point into HTML. Each language should bring its unique strengths to the table whether it be signals, code generation or something else new and exciting.

<br />
<br />

## Ecosystem

### XYZ-Flavored Zero

In the same manner as [GitHub flavored Markdown](https://github.github.com/gfm/), it is encouraged that guest languages bring their own embellishments that take advantage of their syntax's unique features. For example: "Acme flavored Zero."

### Known Implementations

Below is a running list of known guest languages and the various features they support.

| Features                        |   Zero.cs   |
| ------------------------------- | :---------: |
| Dynamic hosting                 |     ✅      |
| SSG (static site generation)    | coming soon |
| AoT (Ahead of time compilation) | coming soon |
| WASM (WebAssembly)              | coming soon |
| WebSockets                      |     ✅      |
| SSE (server-side events)        | coming soon |
| Event-POSTing                   | coming soon |
