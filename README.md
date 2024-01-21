# Zero (the syntax)

Build for the web with zero JavaScript.

In many ways, Xero is like Markdown. It's not just a single implementation but rather a small set of rules for outputting predictable results for the web regardless of platform choice. (Xero could even be implemented in JavaScript, why not? :) Like Markdown, there's also room for various "flavors" to bring their own special embellishments.

Basically, Xero is just vanilla HTML with a few small additions. Its purpose is to reduce the web's dependency on JavaScript by opening the door for other languages to compete. But making JavaScript interchangeable is unrealistic unless other languages can both _generate HTML_ **AND** _manipulate the DOM_ using one common approach. The key is using an HTML-first strategy. Instead of putting HTML inside your logic, Xero puts your logic inside your HTML.

Xero's purpose is grounded in the desire for the web to remain **THE** melting pot of human ideas and progress. The best way to prevent stagnation is to open the floodgates for other languages to compete.

> [!IMPORTANT]
> The examples in this README use a fictional language called `AnyScript` in order to provide concrete examples without favoring any particular language. Conceptually, any imperative language could be substituted.

<br /><br />

## 📂 Components

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

You'll notice the use of the `{{ }}` escape sequence in the component file. You'll learn more about this in the [Hole Punch](https://todo) section below.

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
> A component's attributes are optional by default. The ability to denote some attributes as "required" is outside the scope of this document but is a highly recommended "flavor" to be provided by the implementing language.

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
> Sibling files can technically work with `.js` files too but the recommended approach. This might be useful if you are certain your website will only ever be statically generated. But it's important to note that Xero has a different language-agnostic approach for building dynamic features that offers a gradual migration path from fully static website to fully dynamic web app. You'll learn more about this in the [Dynamic Content](https://todo) section below.

<br />
<br />
<br />
<br />

## 🤰 Children

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

Any tags without a `slot` attribute must default to the reserved slot `content`. Slot order does not matter and any missing slots are treated as `null`, `nil`, `undefined`, etc - however the implementing languages represents missing data.

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
> Slots are optional and if not supplied, the component must still function. Similar to attributes, though, it possible and recommended for an implementing language to support "required inputs" much like a class with a non-nullable property.

<br />
<br />
<br />
<br />

## 🚦 Control Flow

Xero uses a few tags for control flow: `<if>`, `<else>`, `<else-if>` and `<foreach>`. These are reserved keywords so custom components by those names are not allowed. While the content inside these tags might be included, excluded or repeated, the enclosing tag itself is never included in the generated HTML. (The browser wouldn't know what to do with an `<if>` anyway tag right?)

Most other frameworks choose to use their natural syntax instead of extending HTML to handle control flow. For them, this is a sensible choice since it allows for greater flexibility. Since the purpose of Xero is to be language-agnostic, it takes a more generic approach by simply extending HTML and to lean into its [hole-punching](https://todo) approach for extending functionality.

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
<br />
<br />

## 💄 Styles

### Platform and File-Based

CSS can be used normally, including embedded `<style>` tags and externally referenced `.css` files.

However, any `.css` file that shares the same filename and parent directory as an `.html` file is considered a [sibling file](https://todo) and must automatically have its styles included before any HTML to prevent any dreaded [FOUC](https://en.wikipedia.org/wiki/Flash_of_unstyled_content). Inclusion must occur only once per HTML document, since its `.html` counterpart might be repeated multiple times.

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
<br />
<br />

## 🗺️ Routing

### File-Based

Xero uses file-based routing as a language-agnostic way to define your URL routing patterns. Xero does encourage any implementing "flavor" to embellish additional features in their own native language syntax.

### Directories

### Files

### Query String

### `POST`ing

<br />
<br />
<br />
<br />

## 🐵 Migration Path from Static to Dynamic

Astute readers might notice that, at this point, we now have all the tools necessary to host or generate a static website composed of reusable components using only `.html` and `.css` files and nothing else.

This is a good thing. It ties the durability of your frontend to the longevity of the web itself. The more you can "embrace the platform" the less susceptible your codebase is to [software rot](https://en.wikipedia.org/wiki/Software_rot). It shouldn't be an unreasonable expectation to return to a 10 year old project and have everything function exactly as before.

The sections that follow describe the migration path Xero defines to progressively enhance your website with dynamic, reactive and realtime features using languages besides just JavaScript.

## 🤖 BYOL (Bring-Your-Own-Language)

### `<script>` Tags

In the spirit of "embracing the platform" Xero allows the `<script>` tag to be repurposed for any language using the ([to-spec](https://developer.mozilla.org/en-US/docs/Web/HTML/Element/script#language)) `language` attribute.

```xml
my-button.html

<script language="AnyScript">
  count = 0

  func handleClick() {
    count++
  }
</script>

<button onClick={{handleClick}}>
  Clicks: {{count}}
</button>

```

Technically this attribute's possible values were never standardized.  However this is moot since this `<script>` tag 

### Hole Punch

The "hole punch" pattern is familiar since it appears like regular [string interpolation](https://en.wikipedia.org/wiki/String_interpolation). However, in order to excel at both HTML-generation and DOM-manipulation there is one important distinction. Instead of simply filling in the interpolated "holes" and returning a final string, it returns a "composition object" which simply just hangs onto the inputs for later use. Once a composition is built, it becomes trivial to either generate the HTML in full or to compare it with another composition object, input by input, for anything that might have changed and, if so, construct the necessary instructions the DOM needs to updated itself.

This has several advantages:

- Portability
- Speed
- Precision
- Memory
- Derived data

### Event Handlers

### Server vs. Client

<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />
<br />

## 🤖 Dynamic Content

### Hole Punch

Dynamic content is accomplished using the "hole punch" technique. Punch a hole in your declarative code (HTML) for some dynamic content to live using a configurable escape sequence. Any escape sequence is fine so long as it doesn't conflict with typical HTML. This README will use the `{{ }}` escape sequence for its examples.

```xml
my-button.html

<button>
  Clicks: {{count}}
</button>
```

Any expression must be valid, including "derived data."

```xml
my-button.html

<button>
  Hello {{name ?? "friend"}}.  You clicked: {{count * 2}} times.
</button>
```

### BYOL (bring your own language)

There are two ways include dynamic functionality.

1. In a separate file. Use the same file name but your language's natural file extension.
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

```csharp
int count = 0

func handleClick() {
  count++
}
```

</td>
</tr>
</table>

2. In a script tag with the `language` attribute specified.

```xml
<button onClick={{handleClick}}>
  Clicks: {{count}}
</button>

<script language="AnyScript">
  int count = 0

  func handleClick() {
    count++
  }
</script>
```

Xero is intentionally not prescriptive about how a language should reference other code since those conventions differ by language and are already well established.

<br /><br />

## STOP!

## Scripts?

## State
